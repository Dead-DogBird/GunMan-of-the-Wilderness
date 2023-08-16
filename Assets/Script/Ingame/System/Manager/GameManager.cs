using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoSingleton<GameManager>
{
    internal PoolingManager _poolingManager;
    [SerializeField] private GameObject Stage;
    [SerializeField] private GameObject Orbit;
    [SerializeField] private GameObject effectText;
    public PlayerState player { get; private set; }

    void Start()
    {
        _poolingManager.AddPoolingList<Orbit>(450, Orbit);
        _poolingManager.AddPoolingList<EffectText>(10, effectText);
    }

    private void OnEnable()
    {
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
}
