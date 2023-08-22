using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Stage
{
    public GameObject[] map;
}
public class GameManager : MonoSingleton<GameManager>
{
    internal PoolingManager _poolingManager;
   
    [SerializeField] private GameObject Orbit;
    [SerializeField] private GameObject MoveOrbit;
    [SerializeField] private GameObject effectText;
    [SerializeField] private GameObject inGameCoin;
    [SerializeField] private Stage[] _stages;

    [SerializeField] private GameObject[] stores;
    [SerializeField] private GameObject storeManager;

    [SerializeField] private GameObject[] Players;
    public PlayerState player { get; private set; }

    public bool isPlayerDie = false;
    void Start()
    {
        _poolingManager.AddPoolingList<Orbit>(450, Orbit);
        _poolingManager.AddPoolingList<MoveOrbit>(450, MoveOrbit);
        _poolingManager.AddPoolingList<EffectText>(10, effectText);
        _poolingManager.AddPoolingList<InagmeCoin>(50, inGameCoin);
    }
    private void OnEnable()
    {
        player = Instantiate(Players[0], new Vector3(-6, -1), Quaternion.identity).GetComponent<PlayerState>();
        nowStage = Instantiate(_stages[wolrd].map[stage], new Vector3(0, 0), Quaternion.identity);
    }
    void Update()
    {
        
    }
    public void SetPlayer(PlayerState _player)
    {
        if (player != null) return;
        player = _player;
        return;
    }
    public void Effect(Vector3 pos, int count,float random = 0.15f)
    {
        for (int i = 0; i < count; i++)
        {
            _poolingManager.Spawn<Orbit>().Init(new OrbitInfo(true,pos +new Vector3(Random.Range(random,-random),
                Random.Range(random,-random),0.1f),Random.Range(0.7f,2f),0.1f,
                new OrbitColors(new Color(255/255f,255/255f,255/255f,1),new Color(255/255f,0/255f,50/255f,1))),0.45f,true,3);
        }
    }
    public void Effect(Vector3 pos, int count,float scale,OrbitColors colors,bool outlined = true,float outline = 0.2f,float reduce = 4)
    {
        if(outlined)
        {
            for (int i = 0; i < count; i++)
            {
                _poolingManager.Spawn<Orbit>().Init(new OrbitInfo(true,pos +new Vector3(Random.Range(0.15f,-0.15f),
                    Random.Range(0.15f,-0.15f),0.1f),scale+Random.Range(-0.10f,0.10f),0.05f,
                colors),outline,true,reduce);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                _poolingManager.Spawn<Orbit>().Init(new OrbitInfo(true,pos +new Vector3(Random.Range(0.15f,-0.15f),
                        Random.Range(0.15f,-0.15f),0.1f),scale+Random.Range(-0.10f,0.10f),0.05f,
                    colors),true,reduce);
            }
        }
    }
    public void EffectText(Vector3 pos, string text, Color color)
    {
        _poolingManager.Spawn<EffectText>().Init(pos, color, text);
    }
    public void MoveOrbitEffect(Vector3 pos, int count, float scale, OrbitColors colors, bool outlined = true,
        float outline = 0.2f, float reduce = 2,float angle =0,float speed = 10,float randomAngle = 0 )
    {
        if(outlined)
        {
            for (int i = 0; i < count; i++)
            {
                _poolingManager.Spawn<MoveOrbit>().Init(new OrbitInfo(true,pos +new Vector3(Random.Range(0.15f,-0.15f),
                        Random.Range(0.15f,-0.15f),0.1f),scale+Random.Range(-0.10f,0.10f),0.05f,
                    colors),outline,true,reduce,angle+Random.Range(-randomAngle,randomAngle),speed);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                _poolingManager.Spawn<MoveOrbit>().Init(new OrbitInfo(true,pos +new Vector3(Random.Range(0.15f,-0.15f),
                        Random.Range(0.15f,-0.15f),0.1f),scale+Random.Range(-0.10f,0.10f),0.05f,
                    colors),true,reduce,angle+Random.Range(-randomAngle,randomAngle),speed);
            }
        }
        
    }
    public void SpawnCoin(Vector3 pos, int money,float random = 0)
    {
        var randomVector = new Vector3(Random.Range(random,-random),Random.Range(random,-random));
        _poolingManager.Spawn<InagmeCoin>().Init(pos+randomVector,money);

    }
    public void SniperSkill(bool _issniperSkill)
    {
        UIManager.Instance.SniperSkill(_issniperSkill);
        IngameCamera.Instance.SniperSkill(_issniperSkill);
    }

    public int wolrd;
    public int stage;
    private bool isStore = false;
    private GameObject nowStage;
    private GameObject store;
    public void GetNextStage()
    {
        UIManager.Instance.SetFade(true, 0).Forget();
        player.transform.position = new Vector3(-6, -1);
        Destroy(nowStage.gameObject);
        if ((stage == 0 || stage == 2) && (!isStore))
        {
            isStore = true;
            nowStage = Instantiate(stores[wolrd],new Vector3(0,0),Quaternion.identity);
        }
        else
        {
            if (stage == 3)
            {
                wolrd++;
                stage = 0;
            }
            stage++;
            isStore = false;
            nowStage = Instantiate(_stages[wolrd].map[stage], new Vector3(0, 0), Quaternion.identity);
        }
        UIManager.Instance.SetFade(false, 2).Forget();
    }
}
