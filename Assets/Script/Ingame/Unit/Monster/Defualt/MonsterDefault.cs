using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterDefault : MonoBehaviour
{
    [SerializeField] protected float _hp = 100;
    [SerializeField] protected float _speed = 3.5f;

    protected Rigidbody2D _rigid;
    protected bool _targetedPlayer = false;
    public int nextMove;

    private bool isDie = false;
    [SerializeField] private float distanceFromPlayer = 0.1f;
    [SerializeField] ColliderCallbackController colliderCallbackController;

    protected PlayerState player;

    protected Animator _animator;
    protected static readonly int _TargetedPlayer = Animator.StringToHash("TargetedPlayer");

    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        MoveSelect().Forget();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        colliderCallbackController.onColiderEnter += Findedplayer;
        colliderCallbackController.onColiderExit += LosePlayer;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_targetedPlayer)
        {
            NontargetPlayerMove();
        }
        else
        {
            TargetedPlayer();
        }
    }

    private void OnDisable()
    {
        colliderCallbackController.onColiderEnter -= Findedplayer;
        colliderCallbackController.onColiderExit -= LosePlayer;
    }

    private void OnDestroy()
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
    }
    void NontargetPlayerMove()
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
    
    async UniTaskVoid MoveSelect()
    {
        while (!isDie)
        {
            nextMove = Random.Range(-1, 2);
            transform.localScale = nextMove > 0 ? new Vector3(-1,1) : new Vector3(1,1);
            await UniTask.Delay(TimeSpan.FromSeconds(5), ignoreTimeScale: false);
            await UniTask.WaitUntil(() => !_targetedPlayer);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("bullet"))
        {
            Bullet _bullet = other.GetComponent<Bullet>();
            GameManager.Instance.Effect(other.transform.position, 4);
            _hp -= _bullet.damage;
            KnockBack(_bullet.toVector.x,_bullet.damage);
            GameManager.Instance._poolingManager.Despawn(_bullet);
            if (!_targetedPlayer)
            {
                _targetedPlayer = true;
                player = GameManager.Instance.player;
                _animator.SetBool(_TargetedPlayer,true);
            }
            
            if (_hp <= 0)
                Destroy(gameObject);
        }

    }
    async UniTaskVoid WalkParticle()
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
    void Findedplayer(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이를 봤다 안카요");
            player = other.GetComponent<PlayerState>();
            _animator.SetBool(_TargetedPlayer,true);
            _targetedPlayer = true;
            WalkParticle().Forget();
        }
    }
    void KnockBack(float tovector,float damege)
    {
        _rigid.velocity = new Vector2(0,_rigid.velocity.y);
        _rigid.AddForce(new Vector2(tovector*damege*0.5f,0),ForceMode2D.Impulse);
    }
    void LosePlayer(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
          /*  Debug.Log("플레이를 못봤다 안카요");
            player = null;
            _targetedPlayer = false;*/
        }
    }
}
