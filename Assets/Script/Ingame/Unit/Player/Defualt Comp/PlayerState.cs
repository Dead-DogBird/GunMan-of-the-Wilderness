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
    [Header("체력")]
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
            
            if(UIManager.Instance)
            {
                if(value !=0)
                    UIManager.Instance.UpdateGauge(value);
                else
                    UIManager.Instance.ResetGauge(true);
            }
        }
    }

    public float playerMaxHp => _playerMaxHp;
    [Header("스킬관련")]
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
    
    [Header("탄창")]
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
    [Header("공격 관련")]
    [SerializeField] private float _damage;
    [SerializeField] private float _spread;
    [SerializeField] private float _startspread;
    //발사 딜레이
    [SerializeField] private float _fireDelay;
    private float _nowFireDelay;

    //재장선 시간
    [SerializeField] private float _reloadDelay;

    //총알 시간
    [SerializeField] private float _bulletSpeed = 1;

    //총구 위치,색깔
    [SerializeField] private Transform _gunholePos;
    public Transform GetGunholePos
    {
        get
        {
            return _gunholePos;
        }
    }
    [SerializeField] private Color _bulletColor;
    [SerializeField] private Color _orbitColor;
    [SerializeField] private Color _sceorbitColor = new(66 / 255f, 66 / 255f, 95 / 255f, 255 / 255f);
    [SerializeField] private float targetFigure = 0.1f;
    
    //총반동 이펙트 크기
    [SerializeField] private float GuneffectSize = 0.5f;
    [SerializeField] private float GuneffectDegree = 25;
    [SerializeField] private float Guneffectpos = 0.5f;
    public OrbitColors colors
    {
        get { return new OrbitColors(_orbitColor, _sceorbitColor); }
    }
    private FireState_ _fireState;

    public FireState_ FireState => _fireState; 
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
    public PlayerContrl _playerContrl { get; private set; }
    private PlayerMove _playerMove;
    private PlayerUpgrade _playerUpgrade;
    
    public Player_Gun _playerGun { get; private set; }
    [SerializeField] private GameObject Textures;
    private List<SpriteRenderer> _charSprites = new();

    private Animator _animator;
    private bool _isMove;
    private bool _isGround;
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int Ground = Animator.StringToHash("Ground");
    private bool _isimmune = false;
    public bool isResurrection { get; private set; }
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
    public enum FireState_
    {
        Default,
        Reload,
        Delayed,
        NoMag
    };
    private enum PlayerType
    {
        Revolver,
        ShotGun,
        DMR,
        Rifle,
        Sniper,
        Gatling,
        Laser
    };

    [SerializeField] private PlayerType _playerType;
    private void Start()
    {
        _playerContrl = GetComponent<PlayerContrl>();
        _playerGun = GetComponent<Player_Gun>();
        _playerFire = GetComponent<PlayerFire>();
        _playerMove = GetComponent<PlayerMove>();
        _playerUpgrade = GetComponent<PlayerUpgrade>();
        _fireState = FireState_.Default;
        Getsprite(Textures);
        //GameManager.Instance.SetPlayer(this);
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        UIManager.Instance.SetGauge(_playerMaxHp,_playerMaxSkill);
        PlayerHp = _playerMaxHp;
        PlayerSkill = _playerMaxSkill/2;
        UIManager.Instance.SetSkillGaougeColor(_orbitColor);
        UIManager.Instance.UpdateMag(_allMag,true);
        if(_playerType == PlayerType.Sniper)
            SniperPassiveTask().Forget();
        if (_playerType == PlayerType.ShotGun)
            isResurrection = true;
    }
    private void Update()
    {
        if(Input.GetKey(KeyCode.E))
            Debug.Log(_fireState);
        
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
        return (_playerContrl.Userinput.Rkey && _fireState == FireState_.Default);
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
        if (_fireState == FireState_.Reload)
        {
            Debug.Log(_fireState);
            return;
        }
        AudioManager.Instance.PlaySFX(reloadSfxId);
        UIManager.Instance.UpdateMagReload(true,_reloadDelay);
        _fireState = FireState_.Reload;
        for (float i = _reloadDelay; i >= 0; i -= 0.1f)
        {
            if (breakReload)
            {
                breakReload = false;
                break;
            }
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), ignoreTimeScale: isSniperUlt);
        }
        UIManager.Instance.UpdateMagReload(false);
        NowMag = _allMag;
        _fireState = FireState_.Default;
    }

    private bool breakReload=false;
    public void SetMaxMag()
    {
        if (_fireState == FireState_.Reload)
        {
            breakReload = true;
            _fireState = FireState_.Default;
        }
        NowMag = _allMag;
    }
    public void GetDamage(MonsterBullet _monsterBullet)
    {
        Debug.Log("몬스터 불렛 호출됨");
        if (_isimmune) return;
        _isimmune = true;
        GameManager.Instance.Effect(_monsterBullet.transform.position, 4, 0.4f);
        GameManager.Instance.EffectText(_monsterBullet.transform.position,$"-{_monsterBullet.damage}",_monsterBullet.orbitColor.priColor);
        PlayerHp -= _monsterBullet.damage;
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
    void GetDamage(DefaultBossMonster monster)
    {
       
        if (_isimmune) return;
        _isimmune = true;
        
        GameManager.Instance.EffectText(transform.position,$"-{monster.ConnectDamage}",monster.priColor);
        PlayerHp -= monster.ConnectDamage;
        
        GameManager.Instance.Effect(transform.position, 4, 0.4f);
        if (IsDie)
        {
            return;
        }
        GetDamegeTask().Forget();
    }
    void GetDamage_()
    {
      
        if (_isimmune) return;
        _isimmune = true;
        GameManager.Instance.EffectText(transform.position,$"-20",new Color(180/255f,0/255f,230/255f,1));
        PlayerHp -= 20;
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
        AudioManager.Instance.PlaySFX(15,false,1,1);
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
        if (isResurrection)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f), true);
            ResurrectionTask().Forget();
        }
        else
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f), true);
            GameManager.Instance.GameOver();
        }
    }

   
    async UniTaskVoid ResurrectionTask()
    {
        isResurrection = false;
        IsDie = false;
        _playerContrl.enabled = true;
        _playerGun.enabled = true;
        _playerFire.enabled = true;
        _playerMove.enabled = true;
        PlayerHp = _playerMaxHp;
        
        IngameCamera.Instance.transform.position = _diepos+new Vector3(0,0,-10);
        float ypos = 1f;
        IngameCamera.Instance.Shake(0.05f,0.05f,0,1,60);
        Time.timeScale = 0.2f;
        AudioManager.Instance.PlaySFX(20,false,0.5f,-1);
        for (int i = 0; i < 100; i++)
        {
            AudioManager.Instance.SetMusicPitch((float)i/100);
            float angle = Random.Range(0f, 360f); // 0부터 360도 사이의 랜덤한 각도
            float radians = angle * Mathf.Deg2Rad; // 각도를 라디안으로 변환
            float x = 2f * Mathf.Cos(radians); // x 좌표 계산
            float y = 2f * Mathf.Sin(radians);
            GameManager.Instance.MoveOrbitEffect(_diepos+new Vector3(x,y),Random.Range(3,5),0.7f,
                colors, false,0,2,CustomAngle.PointDirection(_diepos,_diepos+new Vector3(x,y))-180,Random.Range(7,12),0);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime), true);
        }
        _rigidbody.simulated = true;
        dead = false;
        IngameCamera.Instance.Shake(0.3f,0.3f,0,1,20);
        transform.localScale = Vector3.one;
        Time.timeScale = 1;
        _isimmune = true;
        for (int i = 0; i < 14; i++)
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

    async UniTaskVoid SniperPassiveTask()
    {
        while (!_isDie)
        {
            PlayerHp += Random.Range(0.0f, 2.0f);
            if (PlayerHp > _playerMaxHp) PlayerHp = _playerMaxHp;
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f), false);
        }
    }
    public GetFireInstance GetFireInstance()
    {
        if (_playerType == PlayerType.Revolver && ((NowMag == 0)||(NowMag == _allMag-1)))
        {
            return new GetFireInstance(transform.position,_gunholePos.position,GetMousePos()-new Vector3(0,0.07f)
                ,_damage*2,_bulletSpeed*1.5f,_bulletColor,new OrbitColors(new Color(255/255f,1/255f,126/255f,1),_sceorbitColor),targetFigure,0);
        }
        return new GetFireInstance(transform.position,_gunholePos.position,GetMousePos()-new Vector3(0,0.07f)
        ,_damage,_bulletSpeed,_bulletColor,new OrbitColors(_orbitColor,_sceorbitColor),targetFigure,_spread);
    }
    public void GetFireEffect()
    {
        _playerGun.FireEffect(Guneffectpos,GuneffectDegree,GuneffectSize);
    }

    public void SetMoney(int _money)
    {
        if (_money > 0 && _playerType == PlayerType.Rifle)
        {
            if (Random.Range(0, 3) ==0)
            {
                Money += _money*2;
                return;
            }
        }
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
        if (other.transform.CompareTag("BossMonster"))
        {
            GetDamage(other.transform.GetComponent<DefaultBossMonster>());
        }

    }

    
    public void skillGauge(float num)
    {
        if (isSniperUlt) return;
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

        if (other.CompareTag("Gate"))
        {
            GameManager.Instance.GetNextStage();
        }
        if (other.CompareTag("BossAttack"))
        {
            GetDamage_();
        }
    }
    public void SniperUlt(bool active)
    {
        _damage *=((active) ? 2 : 0.5f);
        isSniperUlt = active;
        if (isSniperUlt)
        {
            SetMaxMag();
        }
    }

    public void GetKillCallBack()
    {
        if (_playerType == PlayerType.Gatling)
        {
            if (_fireState != FireState_.Reload&&_fireState != FireState_.NoMag)
            {
                NowMag += _allMag / 5;
                if (NowMag > _allMag) NowMag = _allMag;
            }
        }
    }

    public void GetDamegedCallBack()
    {
        if (_playerType == PlayerType.Laser)
        {
            if (Random.Range(0, 100) > 85)
            {
                PlayerHp += 1;
                if (PlayerHp > _playerMaxHp)
                {
                    PlayerHp = _playerMaxHp;
                }
            }
        }
    }
    
    //이하 업그레이드 메서드들
    private int bulletVelocity =0;
    private int reloadspeed = 0;
    private int rpm = 0;
    private int damage = 0;
    private int maxmag = 0;

    public int Upgrade(int UpgradeId)
    {
        if ((bulletVelocity + reloadspeed + rpm + damage + maxmag) >= 25)
            return -2;
        switch (UpgradeId)
        {
            case 0:
            {
                if (bulletVelocity < 5)
                {
                    bulletVelocity++;
                    _bulletSpeed += _playerUpgrade.UpgradeBulletSpeed;
                    return 0;
                }
            }
                break;
            case 1:
            {
                if (reloadspeed < 5)
                {
                    reloadspeed++;
                    _reloadDelay -= _playerUpgrade.UpgradeReloadDelay;
                    return 1;
                }
            }
                break;
            case 2:
            {
                if (rpm < 5)
                {
                    rpm++;
                    _fireDelay -= _playerUpgrade.UpgradeFireDelay;
                    return 2;
                }
            }
                break;
            case 3:
            {
                if (damage < 5)
                {
                    damage++;
                    _damage += _playerUpgrade.UpgradeDamage;
                    return 3;
                }
            }
                break;
            case 4:
            {
                if (maxmag < 5)
                {
                    maxmag++;
                    if (_playerType == PlayerType.ShotGun)
                    {
                        if(maxmag==1||maxmag==3||maxmag==5)
                            _allMag += _playerUpgrade.UpgradeMaxmag;
                    }
                    else 
                        _allMag += _playerUpgrade.UpgradeMaxmag;
                    
                    SetMaxMag();
                    UIManager.Instance.UpdateMag(_allMag,true);
                    return 4;
                }
            }
                break;
        }
        return -1;
    }

    public void UpgradeStat(int UpgradeId)
    {
        switch (UpgradeId)
        {
            case 1:
                PlayerHp = playerMaxHp;
                break;
            case 2:
                _playerMove.UpgradeStat();
                break;
            case 3:
                _playerMove.UpgradeStat(false);
                break;
            case 4:
                float orMaxHp = _playerMaxHp; 
                _playerMaxHp += _playerMaxHp * 0.2f;
                UIManager.Instance.SetGauge(_playerMaxHp,_playerMaxSkill);
                PlayerHp += (_playerMaxHp-orMaxHp);
                break;
            case 5:
                _playerMaxSkill -= 100;
                if (_playerSkill > _playerMaxSkill)
                    PlayerSkill = _playerMaxSkill;
                UIManager.Instance.SetGauge(_playerMaxHp,_playerMaxSkill);
                UIManager.Instance.UpdateGauge(0,_playerSkill);
                break;
            case 6:
                if (!isResurrection)
                    isResurrection = true;
                break;
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
    public float startspread;
    public GetFireInstance(Vector3 playerpos, Vector3 firepos, Vector3 mousepos, float damage, 
        float speed, Color bulletColor, OrbitColors orbitcolors,float targetFigure = 0.1f,float spread = 0,float startspread =0)
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
        this.startspread = startspread;
    }
    
    
}
