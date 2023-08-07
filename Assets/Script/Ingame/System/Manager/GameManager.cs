using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    internal PoolingManager _poolingManager;
    [SerializeField] private GameObject Stage;

    public PlayerState player { get; private set; }

    void Start()
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
