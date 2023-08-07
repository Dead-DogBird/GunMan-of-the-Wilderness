using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
public class IngameCamera : MonoBehaviour
{
    private PlayerState _player;
    private Camera _mainCamera;
    
    
    public float shake_x = 0;
    public float shake_y = 0;
    public float shake_dire = 0;
    public float size = 1;
    public float length = 15;
    
    float camera_size;
    float shakeVol;
    private bool _isPlayerNotNull;

    void Start()
    {
        
        getPlayer().Forget();
        _mainCamera = Camera.main;
        camera_size = _mainCamera.orthographicSize;
    }

    void Update()
    {
        
    }

    private void LateUpdate()
    {
        ShakeUpdate();
        if (_isPlayerNotNull)
        {
            transform.position = _player.transform.position - new Vector3(-5.25f,-2.3f,10);
        }
    }

    void ShakeUpdate()
    {
        _mainCamera.transform.position += new Vector3(Random.Range(-shake_x, shake_x),
            Random.Range(-shake_y, shake_y), -10);
        _mainCamera.transform.localRotation = Quaternion.Euler(0,0,Random.Range(-shake_dire,shake_dire));
        _mainCamera.orthographicSize = camera_size * size;
        
        shake_x -= shake_x / length;
        shake_y -= shake_y / length;
        shake_dire -= shake_dire / length;
        size += (1 - size) / length;
    }

    public void Shake(float x = 0, float y = 0, float dire = 0, float size = 1.5f, float length = 10)
    {
        if (x != 0)
            shake_x = x;
        if (y != 0)
            shake_y = y;
        if (dire != 0)
            shake_dire = dire + ((-dire) * (1 - shakeVol));
        this.size = size + ((1 - size) * (1 - shakeVol));

        this.length = length*0.416f;
    }
    async UniTaskVoid getPlayer()
    {
        await UniTask.WaitUntil(() => GameManager.Instance.player != null);
        _player = GameManager.Instance.player;
        _isPlayerNotNull = _player != null;
    }
}
