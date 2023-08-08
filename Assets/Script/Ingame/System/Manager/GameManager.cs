using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
