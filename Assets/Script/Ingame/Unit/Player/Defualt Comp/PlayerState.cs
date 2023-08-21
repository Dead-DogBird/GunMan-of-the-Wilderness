using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;


// 플레이어의 체력, 기술 게이지, 탄약 등등의 총체적인 상황을 관리
// 충돌 등등에도 관여.
// ReSharper disable once CheckNamespace
public class PlayerState : MonoBehaviour
{
    //체력 및 스킬
    [SerializeField] private float _playerMaxHp = 100;
    [SerializeField] private float _playerHp = 100;
    public float PlayerHp
    {
        get
        {
            return _playerHp;
        }
        private set
        {
            
            _playerHp = value;
            
            if(value !=0)
                UIManager.Instance.UpdateGauge(value);
            else
                UIManager.Instance.ResetGauge(true);
        }
    }
    [SerializeField] private float _playerMaxSkill = 100;
    [SerializeField] private float _playerSkill = 100;
    public float PlayerSkill
    {
        get
        {
            return _playerSkill;
        }
        private set
        {
            _playerSkill = value;
            if(value !=0)
                UIManager.Instance.UpdateGauge(0,value);
            else
                UIManager.Instance.ResetGauge(false,true);
        }
    }
    
    //탄창
    [SerializeField] private int _allMag;
    [SerializeField] private int _nowMag;


    //돈
    private int _money;
    public int Money
    {
        get
        {
            return _money;
        }
        private set
        {
            _money = value;
            UIManager.Instance.UpdateMoney(value);
        }
    }

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
    
    //총 정보
    public int getAllMag => _allMag;
    public int NowMag
    {
        get
        {
            return _nowMag;
        }
        private set
        {
            _nowMag = value;
            UIManager.Instance.UpdateMag(_nowMag);
        }
    }
    public float getDamage => _damage;
    public float getBulletSpeed => _bulletSpeed;

    private bool _isDie = false;
    public bool IsDie
    {
        get
        {
            return _isDie;
        }
        private set
        {
            _isDie = value;
            GameManager.Instance.isPlayerDie = value;
            if (value)
            {
                UIManager.Instance.OnPlayerDie();
            }
        }
    }
    private bool dead = false;

    private PlayerFire _playerFire;
    private PlayerContrl _playerContrl;
    private PlayerMove _playerMove;
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
        _playerFire = GetComponent<PlayerFire>();
        _playerMove = GetComponent<PlayerMove>();
        _fireState = FireState_.Default;
        Getsprite(Textures);
        GameManager.Instance.SetPlayer(this);
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        UIManager.Instance.SetGauge(_playerMaxHp,_playerMaxSkill);
        PlayerHp = _playerMaxHp;
        PlayerSkill = _playerMaxSkill/2;
        UIManager.Instance.SetSkillGaougeColor(_orbitColor);
        UIManager.Instance.UpdateMag(_allMag,true);
    }
    private void Update()
    {
        if (_playerHp <= 0 && !IsDie)
        {
           DieTask().Forget();
        }
    }
    private void Getsprite(GameObject obj)
    {
        for (var i = 0; i < obj.transform.childCount; i++)
        {
            var child = obj.transform.GetChild(i);
            _charSprites.Add(child.GetComponent<SpriteRenderer>());
        }

    }
    public bool GetSkill()
    {
        if (!_playerContrl.Userinput.SkillKey ||
            !(PlayerSkill >= _playerMaxSkill)) return false;

        PlayerSkill = 0;
        return true;
    }
    public bool GetFire()
    {
        if (_playerContrl.Userinput.LeftMouseState &&
            (_fireState == FireState_.Default))
        {
            Fired().Forget();
            return true;
        }
        return false;
    }
    public bool GetReload()
    {
        return (_playerContrl.Userinput.Rkey && (_fireState == FireState_.Default||_fireState == FireState_.Delayed));
    }
    public Vector3 GetMousePos()
    {
        return _playerContrl.Userinput.MousePos;
    }
    public bool isSniperUlt = false;
    // ReSharper disable Unity.PerformanceAnalysis
    private async UniTaskVoid Fired()
    {
        NowMag--;
        _fireState = FireState_.Delayed;
        await UniTask.Delay(TimeSpan.FromSeconds(_fireDelay), ignoreTimeScale: isSniperUlt);
        if(_fireState == FireState_.Delayed)
            _fireState = FireState_.Default;
        if (NowMag <= 0)
        {
            _fireState = FireState_.NoMag;
            ReLoad().Forget();
        }
    }
    [SerializeField] protected int reloadSfxId;
    // ReSharper disable Unity.PerformanceAnalysis
    public async UniTaskVoid ReLoad()
    {
        AudioManager.Instance.PlaySFX(reloadSfxId);
        UIManager.Instance.UpdateMagReload(true,_reloadDelay);
        _fireState = FireState_.Reload;
        await UniTask.Delay(TimeSpan.FromSeconds(_reloadDelay), ignoreTimeScale: isSniperUlt);
        UIManager.Instance.UpdateMagReload(false);
        NowMag = _allMag;
        _fireState = FireState_.Default;
    }
    public void SetMaxMag()
    {
        NowMag = _allMag;
    }
    public void GetDamage(MonsterBullet _monsterBullet)
    {
        if (_isimmune) return;

        _isimmune = true;
        AudioManager.Instance.PlaySFX(15,false,1,1);
        GameManager.Instance.Effect(_monsterBullet.transform.position, 4, 0.4f);
        GameManager.Instance.EffectText(_monsterBullet.transform.position,$"-{_monsterBullet.damage}",_monsterBullet.orbitColor.priColor);
        PlayerHp -= _monsterBullet.damage;
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
            PlayerHp -= monster.damage/2;
        }
        else
        {
            GameManager.Instance.EffectText(transform.position,$"-10",new Color(180/255f,0/255f,230/255f,1));
            PlayerHp -= 10;
        }
        GameManager.Instance.Effect(transform.position, 4, 0.4f);
        if (IsDie)
        {
            
            return;
        }
        GetDamegeTask().Forget();
    }
    async UniTaskVoid GetDamegeTask()
    {
        UIManager.Instance.PlayerHit();
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
    private Vector3 _diepos;
    async UniTaskVoid DieTask()
    {
        IsDie = true;
        _rigidbody.simulated = false;
        _playerContrl.enabled = false;
        _playerGun.enabled = false;
        _playerFire.enabled = false;
        _playerMove.enabled = false;
        _diepos = transform.position;
        IngameCamera.Instance.transform.position = _diepos+new Vector3(0,0,-10);
        float ypos = 1f;
        IngameCamera.Instance.Shake(0.05f,0.05f,0,1,60);
        Time.timeScale = 0.2f;
        AudioManager.Instance.PlaySFX(20);
        for (int i = 0; i < 100; i++)
        {
            AudioManager.Instance.SetMusicPitch((float)(100-i)/100);
            GameManager.Instance.MoveOrbitEffect(transform.position+new Vector3(0,0.5f),Random.Range(3,5),0.7f,
                colors, false,0,2, 180,Random.Range(7,12),180);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime), true);
        }
        dead = true;
        GameManager.Instance.MoveOrbitEffect(transform.position+new Vector3(0,0.5f),Random.Range(3,5),2f,
            colors, false,0,2, 180,Random.Range(7,12),180);
        IngameCamera.Instance.Shake(0.3f,0.3f,0,1,20);
        transform.localScale = Vector3.zero;
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), true);
        Time.timeScale = 0.1f;
    }
    
    public GetFireInstance GetFireInstance()
    {
        
        return new GetFireInstance(transform.position,_gunholePos.position,GetMousePos()-new Vector3(0,0.07f)
        ,_damage,_bulletSpeed,_bulletColor,new OrbitColors(_orbitColor,_sceorbitColor),targetFigure,_spread);
    }
    public void GetFireEffect()
    {
        _playerGun.FireEffect();
    }

    public void SetMoney(int _money)
    {
        Money += _money;
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
        PlayerSkill += num;
        if (PlayerSkill > _playerMaxSkill)
            PlayerSkill = _playerMaxSkill;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MonsterBullet"))
        {
            GetDamage(other.transform.GetComponent<MonsterBullet>());
        }
        if (other.CompareTag("Coin"))
        {
            other.GetComponent<InagmeCoin>().Getplayer(this);
        }
    }
    public void SniperUlt(bool active)
    {
        _damage *=((active) ? 2 : 0.5f);
        isSniperUlt = active;
        if (isSniperUlt)
        {
            NowMag = _allMag;
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
