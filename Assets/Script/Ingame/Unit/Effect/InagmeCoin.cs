using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;

public class InagmeCoin : PoolableObj
{
    [SerializeField] private GameObject Particle;
    [SerializeField] private GameObject Shadow;
    private int _money;

    private Vector3 tVector3;

    private bool grounded = false;
    private bool ismovetoPlayer;
    
    private float ypos;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnEnable()
    {
        ReleseReserv(20).Forget();
        grounded = false;
        ismovetoPlayer = false;
        Shadow.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (grounded && !ismovetoPlayer && (transform.position.y - tVector3.y) > 0.01f)
        {
            transform.position += new Vector3(0, ypos, 0);
            if((transform.position.y-tVector3.y) > 0.01f)
                ypos += (-0.15f-ypos)*(Time.unscaledDeltaTime*7);
            if((transform.position.y - tVector3.y) <= 0.01f)
                Shadow.SetActive(true);
        }
        if (ismovetoPlayer)
        {
            transform.position += ( _player.transform.position-transform.position)*(Time.deltaTime*16);
            if (Vector2.Distance(_player.transform.position, transform.position) < 0.5f)
            {
                GameManager.Instance._poolingManager.Despawn(this);
            }
        }
    }

    private void FixedUpdate()
    {
        
    }

    public GameObject Init(Vector3 pos, int money)
    {
        transform.position = pos+new Vector3(0,1f);
        _money = money;
        ypos = 0.15f;
        RaycastHit2D ground = Physics2D.Raycast(pos, Vector2.down, 10,
            LayerMask.GetMask("Platform"));
        if (ground)
        {
            grounded = true;
            tVector3 = new Vector3(pos.x, ground.point.y+0.3f);
        }
        CoinTask().Forget();
        return gameObject;
    }

    private PlayerState _player;
    public void Getplayer(PlayerState player)
    {
        if (ismovetoPlayer) return;
        
        _player = player;
        ismovetoPlayer = true;
        Shadow.SetActive(false);
    }

    void OnDisable()
    {
        if (ismovetoPlayer)
        {
            _player.SetMoney(_money);
            var random = Random.Range(2, 6);
            AudioManager.Instance.PlaySfXDelayed(18,0.1f*random,false,0.6f,1);
            for(int i =0;i<random;i++)
            {
                AudioManager.Instance.PlaySfXDelayed(18,0.2f,false,0.6f,1);
                var paticle = Instantiate(Particle);
                paticle.transform.position = transform.position + new Vector3(Random.Range(0.3f,-0.3f),Random.Range(0.3f,-0.3f));
                paticle.transform.localScale *= Random.Range(0.5f,0.8f); 
                Destroy(paticle.gameObject,0.2f);
            }            
        }
    }

    async UniTaskVoid CoinTask()
    {
        while (gameObject.activeSelf)
        {
            var paticle = Instantiate(Particle);
            paticle.transform.position = transform.position + new Vector3(Random.Range(0.3f,-0.3f),Random.Range(0.3f,-0.3f));
            paticle.transform.localScale *= Random.Range(0.2f,0.7f); 
            Destroy(paticle.gameObject,0.2f);
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), ignoreTimeScale: false);
        }
    }
    protected override async UniTaskVoid ReleseReserv(float delay = 2)
    {
        while(gameObject.activeSelf)
        {
            delay -= 0.1f;
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), ignoreTimeScale: false);
            if (delay <= 0)
                break;
        }
    }
}
