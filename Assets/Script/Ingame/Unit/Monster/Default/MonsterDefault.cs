using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Mono.Cecil.Cil;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterDefault : MonoBehaviour
{
    [SerializeField] protected float _hp = 100;
    [SerializeField] protected float _speed = 3.5f;

    protected Rigidbody2D _rigid;
    public bool _targetedPlayer { get; protected set; }
    protected int nextMove;

    public bool isDie { get; protected set; }
    [SerializeField] protected float distanceFromPlayer = 0.1f;
    [SerializeField] ColliderCallbackController colliderCallbackController;

    public PlayerState player{ get; protected set; }
    
    public float bulletSpeed;
    protected Animator _animator;
    protected static readonly int _TargetedPlayer = Animator.StringToHash("TargetedPlayer");

    [CanBeNull] protected MonsterDefaultAttack _attack;

    [SerializeField] private int hitSfxId = 16;
    [SerializeField] private int dieSfxId = 21;


    // Start is called before the first frame update
    protected void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        MoveSelect().Forget();
        _animator = GetComponent<Animator>();
        _attack = GetComponent<MonsterDefaultAttack>();
    }

    protected void OnEnable()
    {
        colliderCallbackController.onColiderEnter += Findedplayer;
        colliderCallbackController.onColiderExit += LosePlayer;
        _hp +=_hp*(GameManager.Instance.wolrd * 0.7f);
    }

    // Update is called once per frame
    protected void Update()
    {
        if (_hp <= 0)
            Die();
        if (!_targetedPlayer)
        {
            NontargetPlayerMove();
        }
        else
        {
            TargetedPlayer();
        }
    }

    protected void OnDisable()
    {
        colliderCallbackController.onColiderEnter -= Findedplayer;
        colliderCallbackController.onColiderExit -= LosePlayer;
    }

    protected void OnDestroy()
    {
        isDie = true;
    }

    protected virtual void TargetedPlayer()
    {
        if (Mathf.Abs(transform.position.x - player.transform.position.x) >= distanceFromPlayer)
        {
            transform.Translate(new Vector3(_speed*(player.transform.position.x > transform.position.x? 2 : -2), 0) * Time.deltaTime);
        }
        if (player.transform.position.x < transform.position.x&&transform.localScale.x<0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (player.transform.position.x > transform.position.x&&transform.localScale.x>0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }   
        var frontVec = new Vector2(_rigid.position.x + (transform.localScale.x>0 ? -0.5f:0.5f),_rigid.position.y+0.2f);
        Debug.DrawLine(frontVec,frontVec+new Vector2(0,1),Color.red);
        if (Physics2D.Raycast(frontVec, Vector3.up, 1, LayerMask.GetMask("Platform")))
        {
            _rigid.AddForce(new Vector2(0,1),ForceMode2D.Impulse);
        }

        var obj =  Physics2D.Raycast(frontVec, Vector3.up, 1);
        if(obj)
        {
            if (obj.transform.CompareTag("Monster"))
            {
                _rigid.AddForce(new Vector2(0,1),ForceMode2D.Impulse);
            }
        }
    }
    protected void NontargetPlayerMove()
    {
        _rigid.velocity = new Vector2(nextMove, _rigid.velocity.y);
        Vector2 frontVec = new Vector2(_rigid.position.x + nextMove*0.2f,_rigid.position.y);
        if (!Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform")))
        {
            nextMove *= -1;
            transform.localScale = nextMove > 0 ? new Vector3(-1,1) : new Vector3(1,1);
        }
        frontVec = new Vector2(_rigid.position.x + nextMove*0.2f,_rigid.position.y+0.5f);
        if (Physics2D.Raycast(frontVec, Vector3.up, 1, LayerMask.GetMask("Platform")))
        {
            nextMove *= -1;
            transform.localScale = nextMove > 0 ? new Vector3(-1,1) : new Vector3(1,1);
        }
        
    }
    
    protected async UniTaskVoid MoveSelect()
    {
        while (!isDie)
        {
            nextMove = Random.Range(-1, 2);
            transform.localScale = nextMove > 0 ? new Vector3(-1,1) : new Vector3(1,1);
            await UniTask.Delay(TimeSpan.FromSeconds(5), ignoreTimeScale: false);
            await UniTask.WaitUntil(() => !_targetedPlayer);
        }
    }

    public void OnDameged(DamageInfo dmgInfo,bool isLaseAttack = false)
    {
        AudioManager.Instance.PlaySFX(hitSfxId);
        GameManager.Instance.player.GetDamegedCallBack();
        GameManager.Instance.Effect(dmgInfo.pos, 4,0.4f);
        GameManager.Instance.EffectText(dmgInfo.pos,$"{dmgInfo.Damage}",dmgInfo.color);
        _hp -= dmgInfo.Damage;
        GameManager.Instance.MoveOrbitEffect(dmgInfo.pos,Random.Range(5,7),1f,
            new OrbitColors(dmgInfo.color,dmgInfo.color),
            false,0,2, Random.Range(0f,360f),Random.Range(7,12));
        if(isLaseAttack)
            GameManager.Instance.player.skillGauge(dmgInfo.Damage);
        if (!_targetedPlayer)
        {
            SetTargetPlayer(GameManager.Instance.player);
        }
    }
    protected void onDamaged(Bullet _bullet)
    {
        GameManager.Instance.player.GetDamegedCallBack();
        AudioManager.Instance.PlaySFX(hitSfxId);
        GameManager.Instance.Effect(_bullet.transform.position, 4,0.4f);
        GameManager.Instance.EffectText(_bullet.transform.position,$"{_bullet.damage}",_bullet.orbitColor.priColor);
        _hp -= _bullet.damage;
        KnockBack(_bullet.toVector,_bullet.damage);
        GameManager.Instance._poolingManager.Despawn(_bullet);
        GameManager.Instance.player.skillGauge(_bullet.damage);
        if (!_targetedPlayer)
        {
            SetTargetPlayer(GameManager.Instance.player);
        }
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
    protected async UniTaskVoid WalkParticle()
    {
        while (!isDie&&_targetedPlayer)
        {
            float radom = Random.Range(200f/255f, 255f/255f);
            var _transform = transform;
            GameManager.Instance.Effect(_transform.position+new Vector3(_transform.localScale.x>0 ? 0.5f: -0.5f,0),Random.Range(1,4),0.7f,
                new OrbitColors(new Color(radom,radom,radom,1),new Color(radom,radom,radom,1)),false,0,2);
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f),ignoreTimeScale:false);
        }
    }
    protected void Findedplayer(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetTargetPlayer(other.GetComponent<PlayerState>());
        }
    }
    protected void KnockBack(Vector3 tovector,float damege)
    {
        _rigid.velocity = new Vector2(0,_rigid.velocity.y);
        var pos = (transform.position - tovector);
        pos.Normalize();
        float clamped = Mathf.Clamp(damege, 8, 200);
        clamped = (clamped-8)/192;
        clamped = Mathf.Lerp(0.5f, 1, clamped);
        
        _rigid.AddForce(new Vector2(tovector.x*clamped,0),ForceMode2D.Impulse);
    }
    protected void LosePlayer(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
      
        }
    }

    [SerializeField] private int dropMoney = 100;
    protected void Die()
    {
        isDie = true;
        AudioManager.Instance.PlaySFX(dieSfxId);
        GameManager.Instance.Effect(transform.position, 4,0.4f);  
        GameManager.Instance.player.skillGauge(100/GameManager.Instance.player.getDamage);
        GameManager.Instance.SpawnCoin(transform.position, dropMoney);
        GameManager.Instance.player.GetKillCallBack();
        Destroy(gameObject);
    }
    protected virtual void SetTargetPlayer(PlayerState _player)
    {
        if (_targetedPlayer) return;
        
        player = _player;
        _animator.SetBool(_TargetedPlayer,true);
        _targetedPlayer = true;
        WalkParticle().Forget();
        _attack?.startAttack();
    }


}

public class DamageInfo
{
    public float Damage;
    public Color color;
    public Vector3 pos;
    public DamageInfo(float _damage, Color _color,Vector3 _pos)
    {
        Damage = _damage;
        color = _color;
        pos = _pos;
    }
    
}
