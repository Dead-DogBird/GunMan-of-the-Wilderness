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
    public PlayerState player { get; private set; }

    void Start()
    {
        _poolingManager.AddPoolingList<Orbit>(50, Orbit);
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

    public void Effect(Vector3 pos, int count)
    {
        for (int i = 0; i < count; i++)
        {
            _poolingManager.Spawn<Orbit>().Init(new OrbitInfo(true,pos +new Vector3(Random.Range(0.15f,-0.15f),
                Random.Range(0.15f,-0.15f),0.1f),Random.Range(0.7f,2f),0.1f,
                new OrbitColors(new Color(255/255f,255/255f,255/255f,1),new Color(255/255f,0/255f,50/255f,1))),0.45f,true,4);
        }
    }
    public void Effect(Vector3 pos, int count,float scale,OrbitColors colors)
    {
        for (int i = 0; i < count; i++)
        {
            _poolingManager.Spawn<Orbit>().Init(new OrbitInfo(true,pos +new Vector3(Random.Range(0.15f,-0.15f),
                    Random.Range(0.15f,-0.15f),0.1f),scale+Random.Range(-0.10f,0.10f),0.05f,
                colors),0.2f,true,4);
        }
    }
}
