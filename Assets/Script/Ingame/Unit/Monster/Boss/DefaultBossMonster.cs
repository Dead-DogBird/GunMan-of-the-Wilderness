using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class DefaultBossMonster : MonoBehaviour
{
    
    [SerializeField] protected float _hp = 20000;
    [SerializeField] protected float _speed = 3.5f;
    [SerializeField] protected Color BulletColor;
    [SerializeField] public Color priColor,secColor;
    
    
    [SerializeField] private int hitSfxId = 16;
    [SerializeField] private int dieSfxId = 21;

    [SerializeField] public float ConnectDamage;
    [SerializeField] protected GameObject Gate;
    public float SkillDamage;
    public bool isDie { get; protected set; } = false;

    protected OrbitColors colors => new(priColor, secColor);

    public float BossHp => _hp;
    
    protected delegate UniTaskVoid BossPattern();
    protected BossPattern[] BossPatterns;
    protected Rigidbody2D _rigid;
    protected PlayerState _player;
    
    
    protected bool isBossPattern = true;

    protected GameObject instGate;
    // Start is called before the first frame update
    protected void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        UIManager.Instance.SetBossUI(this);
        _player = GameManager.Instance.player;
        EnterDelay().Forget();
        isDie = false;
    }

    private int random;
    // Update is called once per frame
    protected void Update()
    {
        if(_hp<=0&&!isDie)
            DieTask().Forget();
        if (!isBossPattern)
        {
            random = Random.Range(0, BossPatterns.Length);
            BossPatterns[random]().Forget();
            Debug.Log(random);
        }
    }

    protected void OnDestroy()
    {
        if(instGate)
            Destroy(instGate.gameObject);
    }

    protected async UniTaskVoid EnterDelay()
    {
        isBossPattern = true;
        float orispeed = _speed;
        _speed = 0;
        IngameCamera.Instance.FocusBoss(true,transform.position);
        await UniTask.Delay(TimeSpan.FromSeconds(3), false);
        IngameCamera.Instance.FocusBoss(false,transform.position);
        _speed = orispeed;
        isBossPattern = false;
    }
    protected virtual async UniTaskVoid DieTask()
    {
        isDie = true;
        GetComponent<BoxCollider2D>().enabled = false;
        Time.timeScale = 0.3f;
        IngameCamera.Instance.Shake(0.05f,0.05f,0,1,60);
        await UniTask.Delay(TimeSpan.FromSeconds(0.4f),true);
        AudioManager.Instance.PlaySFX(20);
        for (int i = 0; i < 100; i++)
        {
            GameManager.Instance.MoveOrbitEffect(transform.position+new Vector3(0,0.5f),Random.Range(3,5),1.5f,
                colors, false,0,2, 180,Random.Range(7,12),180);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime), true);
        }
        IngameCamera.Instance.Shake(0.3f,0.3f,0,1,20);
        transform.localScale = Vector3.zero;
        Time.timeScale = 1f;
        for (int i = 0; i < 10; i++)
        {
            GameManager.Instance.SpawnCoin(transform.position+new Vector3(Random.Range(-3.0f,3.0f),0), 200);
        }
        instGate = Instantiate(Gate,new Vector3(75,0), quaternion.identity);
        UIManager.Instance.SetBossUI();
        
    }
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("bullet"))
        {
            onDamaged(other.GetComponent<Bullet>());
        }
        if (other.CompareTag("LaserAttack"))
        {
            OnDameged(new DamageInfo(GameManager.Instance.player.getDamage,GameManager.Instance.player.colors.priColor,transform.position),true);
        }
        if (other.CompareTag("DMRUlt"))
        {
            OnDameged(new DamageInfo(GameManager.Instance.player.getDamage,GameManager.Instance.player.colors.priColor,transform.position));
        }
        if (other.CompareTag("Missle"))
        {
            other.GetComponent<RevolverRoket>().Explosion();
        }
        if (other.CompareTag("LasertUlt"))
        {
            
            OnDameged(new DamageInfo(GameManager.Instance.player.getDamage*15,GameManager.Instance.player.colors.priColor,transform.position));
        }
        if (other.CompareTag("RifleUlt"))
        {
            OnDameged(new DamageInfo(GameManager.Instance.player.getDamage*2.5f,GameManager.Instance.player.colors.priColor,transform.position));
        }
        if (other.CompareTag("ShotGunUlt"))
        {
            OnDameged(new DamageInfo(GameManager.Instance.player.getDamage*3.5f,GameManager.Instance.player.colors.priColor,transform.position));
        }
    }
    public void OnDameged(DamageInfo dmgInfo,bool isLaseAttack = false)
    {
        AudioManager.Instance.PlaySFX(hitSfxId);
        GameManager.Instance.Effect(dmgInfo.pos, 4,0.4f);
        GameManager.Instance.EffectText(dmgInfo.pos,$"{dmgInfo.Damage}",dmgInfo.color);
        _hp -= dmgInfo.Damage;
        GameManager.Instance.MoveOrbitEffect(dmgInfo.pos,Random.Range(5,7),1f,
            new OrbitColors(dmgInfo.color,dmgInfo.color),
            false,0,2, Random.Range(0f,360f),Random.Range(7,12));
        if(isLaseAttack)
            GameManager.Instance.player.skillGauge(dmgInfo.Damage);
    }
    protected void onDamaged(Bullet _bullet)
    {
        AudioManager.Instance.PlaySFX(hitSfxId);
        GameManager.Instance.Effect(_bullet.transform.position, 4,0.4f);
        GameManager.Instance.EffectText(_bullet.transform.position,$"{_bullet.damage}",_bullet.orbitColor.priColor);
        _hp -= _bullet.damage;
        GameManager.Instance._poolingManager.Despawn(_bullet);
        GameManager.Instance.player.skillGauge(_bullet.damage);
    }
}
