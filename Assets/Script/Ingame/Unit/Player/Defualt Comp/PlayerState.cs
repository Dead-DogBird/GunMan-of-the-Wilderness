using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;


// 플레이어의 체력, 기술 게이지, 탄약 등등의 총체적인 상황을 관리
// 충돌 등등에도 관여.
// ReSharper disable once CheckNamespace
public class PlayerState : MonoBehaviour
{
    //체력
    [SerializeField] private float _playerHp = 100;

    //탄창
    [SerializeField] private int _allMag;
    [SerializeField] private int _nowMag;

    //스킬
    private float _skillgauge = 100;
    private float _nowskillgauge = 100;

    //돈
    private int _money;

    //대미지
    [SerializeField] private float _damage;

    [SerializeField] private float _spread;

    //발사 딜레이
    [SerializeField] private float _fireDelay;
    private float _nowFireDelay;

    //재장선 시간
    [SerializeField] private float _reloadDelay;

    //총알 시간
    [SerializeField] private float _bulletSpeed = 1;

    //총구 위치,색깔
    [SerializeField] private Transform _gunholePos;
    [SerializeField] private Color _bulletColor;
    [SerializeField] private Color _orbitColor;
    [SerializeField] private Color _sceorbitColor = new(66 / 255f, 66 / 255f, 95 / 255f, 255 / 255f);
    [SerializeField] private float targetFigure = 0.1f;
    public OrbitColors colors
    {
        get { return new OrbitColors(_orbitColor, _sceorbitColor); }
    }
    private FireState_ _fireState;
    public float getPlayerHp => _playerHp;
    public int getAllMag => _allMag;
    public float getSkillgauge => _skillgauge;
    public float getNowSkillgauge => _nowskillgauge;
    public int getMoney => _money;
    public int getNowMag => _nowMag;
    public float getDamage => _damage;
    public float getBulletSpeed => _bulletSpeed;

    private PlayerContrl _playerContrl;
    public Player_Gun _playerGun { get; private set; }
    [SerializeField] private GameObject Textures;
    private List<SpriteRenderer> _charSprites = new();

    private Animator _animator;
    private bool _isMove;
    private bool _isGround;
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int Ground = Animator.StringToHash("Ground");
    private bool _isimmune = false;

    private Rigidbody2D _rigidbody;

    public bool isMove
    {
        get => _isMove;

        set
        {
            if (_isMove != value)
            {
                _isMove = value;
                _animator.SetBool(Move, _isMove);
            }
        }

    }

    public bool isGround
    {
        get => _isGround;

        set
        {
            if (_isGround != value)
            {
                _isGround = value;
                _animator.SetBool(Ground, _isGround);
            }
        }

    }


    private enum FireState_
    {
        Default,
        Reload,
        Delayed,
        NoMag
    };

    private void Start()
    {
        _playerContrl = GetComponent<PlayerContrl>();
        _playerGun = GetComponent<Player_Gun>();
        _fireState = FireState_.Default;
        Getsprite(Textures);
        GameManager.Instance.SetPlayer(this);
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Getsprite(GameObject obj)
    {
        for (var i = 0; i < obj.transform.childCount; i++)
        {
            var child = obj.transform.GetChild(i);
            _charSprites.Add(child.GetComponent<SpriteRenderer>());
        }

    }

    private void Update()
    {


    }

    public bool GetSkill()
    {
        if (!_playerContrl.Userinput.SkillKey ||
            !(_nowskillgauge >= _skillgauge)) return false;

        _nowskillgauge = 0;
        return true;
    }

    private FireState_ GetFireCommand()
    {
        switch (_fireState)
        {
            case FireState_.Reload:
                return _fireState;
            case FireState_.Default when _nowMag <= 0:
                _fireState = FireState_.NoMag;
                ReLoad().Forget();
                return _fireState;
            case FireState_.Default:
                _nowMag--;
                Fired().Forget();
                return FireState_.Default;
            default:
                return _fireState;
        }
    }

    public bool GetFire()
    {
        return _playerContrl.Userinput.LeftMouseState &&
               (GetFireCommand() == FireState_.Default);
    }

    public Vector3 GetMousePos()
    {
        return _playerContrl.Userinput.MousePos;
    }

    public bool isSniperUlt = false;
    private async UniTaskVoid Fired()
    {
        _fireState = FireState_.Delayed;
        await UniTask.Delay(TimeSpan.FromSeconds(_fireDelay), ignoreTimeScale: isSniperUlt);
        _fireState = FireState_.Default;
    }

    private async UniTaskVoid ReLoad()
    {
        _fireState = FireState_.Reload;
        await UniTask.Delay(TimeSpan.FromSeconds(_reloadDelay), ignoreTimeScale: isSniperUlt);
        _nowMag = _allMag;
        _fireState = FireState_.Default;
    }

    public void GetDamage(MonsterBullet _monsterBullet)
    {
        if (_isimmune) return;

        _isimmune = true;
        GameManager.Instance.Effect(_monsterBullet.transform.position, 4, 0.4f);
        GameManager.Instance.EffectText(_monsterBullet.transform.position,$"-{_monsterBullet.damage}",_monsterBullet.orbitColor.priColor);
        _playerHp -= _monsterBullet.damage;
        //KnockBack(_monsterBullet.toVector.x,_monsterBullet.damage);
        GameManager.Instance._poolingManager.Despawn(_monsterBullet);
        GetDamegeTask().Forget();
    }

    void GetDamage(MonsterDefaultAttack monster)
    {
        if (_isimmune) return;
        _isimmune = true;
        if (monster is not null)
        {
            GameManager.Instance.EffectText(transform.position,$"-{monster.damage/2}",monster.PriColor);
            _playerHp -= monster.damage/2;
        }
        else
        {
            GameManager.Instance.EffectText(transform.position,$"-10",new Color(180/255f,0/255f,230/255f,1));
            _playerHp -= 10;
        }
        UIManager.Instance.PlayerHit();
        GameManager.Instance.Effect(transform.position, 4, 0.4f);
        GetDamegeTask().Forget();
    }

    async UniTaskVoid GetDamegeTask()
    {
        IngameCamera.Instance.Shake(0.1f,0.1f,3,1.1f,10);
        
        Time.timeScale = 0;
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f), ignoreTimeScale: true);
        Time.timeScale = 1;
        
        
        for (int i = 0; i < 7; i++)
        {
            foreach (var sprite in _charSprites)
            {
                sprite.color = new Color(1,1,1,(i % 2 == 0) ? 0.5f : 1);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), ignoreTimeScale: false);
        }
        foreach (var sprite in _charSprites)
        {
            sprite.color = new Color(1,1,1,1);
        }
        _isimmune = false;
    }

    public GetFireInstance GetFireInstance()
    {
        
        return new GetFireInstance(transform.position,_gunholePos.position,GetMousePos()
        ,_damage,_bulletSpeed,_bulletColor,new OrbitColors(_orbitColor,_sceorbitColor),targetFigure,_spread);
    }

    public void GetFireEffect()
    {
        _playerGun.FireEffect();
    }

    public void PlayAnimation(string name, int layer, float normalizedtime)
    {
        _animator.Play(name,layer,normalizedtime);
        
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Monster"))
        {
            GetDamage(other.transform.GetComponent<MonsterDefaultAttack>());
        }
    }

    public void skillGauge(float num)
    {
        _nowskillgauge += num;
        if (_nowskillgauge > _skillgauge)
            _nowskillgauge = _skillgauge;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MonsterBullet"))
        {
            GetDamage(other.transform.GetComponent<MonsterBullet>());
        }
    }
    public void SniperUlt(bool active)
    {
        _damage *=((active) ? 2 : 0.5f);
        isSniperUlt = active;
        if (isSniperUlt)
        {
            _nowMag = _allMag;
        }
    }
}

public struct GetFireInstance
{
    public Vector3 firepos;
    public Vector3 mousepos;
    public float damage;
    public float speed;
    public Color bulletColor;
    public OrbitColors orbitcolors;
    public float spread;
    public Vector3 playerpos;
    public float targetFigure;
    public GetFireInstance(Vector3 playerpos, Vector3 firepos, Vector3 mousepos, float damage, 
        float speed, Color bulletColor, OrbitColors orbitcolors,float targetFigure = 0.1f,float spread = 0)
    {
        this.playerpos = playerpos;
        this.firepos = firepos;
        this.mousepos = mousepos;
        this.damage = damage;
        this.speed = speed;
        this.bulletColor = bulletColor;
        this.orbitcolors = orbitcolors;
        this.spread = spread;
        this.targetFigure = targetFigure;
    }
    
    
}
