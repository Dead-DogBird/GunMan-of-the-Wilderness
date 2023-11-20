using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
public class IngameCamera : MonoSingleton<IngameCamera>
{
    private PlayerState _player;
    private Camera _camera;
    [SerializeField] private float smooth = 5;
    
    public float shake_x = 0;
    public float shake_y = 0;
    public float shake_dire = 0;
    public float size = 1;
    public float length = 15;
    
    float camera_size;
    float shakeVol;
    private float power = 5;
    public Vector2 position;
    
    private float lastTime;
    private const float OneFrameDeltaTime = 1 / 120f;

    private bool isFocusBoss = false;
    private Vector3 bossPos;
    void Start()
    {
        
        getPlayer().Forget();
        _camera = GetComponent<Camera>();
        camera_size = _camera.orthographicSize;
        lastTime = Time.unscaledTime;
        power *= PlayerPrefs.GetFloat("ShakePower");
    }

    void Update()
    {
       

    }

    private void LateUpdate()
    {
        if (GameManager.Instance.isPlayerDie)
        {
            ShakeUpdate();
            return;
        }

        var player_position = _player.transform.position;
       
        if(isSniperSkill)
            player_position = Vector3.Lerp(player_position,Camera.main.ScreenToWorldPoint(Input.mousePosition),0.5f)-new Vector3(0,0,10);
        else
        {
            player_position = Vector3.Lerp(player_position,Camera.main.ScreenToWorldPoint(Input.mousePosition),0.2f)-new Vector3(0,0,10);
        }
        if(isFocusBoss)
            player_position = bossPos;

        if (player_position.y < -2.49)
            player_position.y = -2.49f;
        position += (Vector2)((player_position - transform.position) / smooth);
        ShakeUpdate();
    }

    void ShakeUpdate()
    {
        transform.position = new Vector3(position.x + Random.Range(-shake_x, shake_x), position.y + Random.Range(-shake_y, shake_y), transform.position.z);
        transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-shake_dire, shake_dire));
        _camera.orthographicSize = camera_size * size;

        shake_x -= shake_x / length;
        shake_y -= shake_y / length;
        shake_dire -= shake_dire / length;
        if (size != 1)
        {
            size += (1 - size) / length;
            if ((1 - size).Abs() <= 0.0001f)
                size = 1;
        }
    }

    public void Shake(float x = 0, float y = 0, float dire = 0, float size = 1.5f, float length = 10)
    {
        if (shake_x < x * power)
            shake_x = x * power;
        if (shake_y < y * power)
            shake_y = y * power;
        if (shake_dire < dire)
            shake_dire = dire;
        if (this.size > 1 - (1 - size) * power)
            this.size = 1 - (1 - size) * power;
        this.length = length;
        ShakeUpdate();
    }
    async UniTaskVoid getPlayer()
    {
        await UniTask.WaitUntil(() => GameManager.Instance.player != null);
        _player = GameManager.Instance.player;
    }
    private bool isSniperSkill;
    public void SniperSkill(bool _isSniperSkill)
    {
        isSniperSkill = _isSniperSkill;
    }
    public void FocusBoss(bool isfocus = true, Vector3 pos = new Vector3())
    {
        isFocusBoss = isfocus;
        bossPos = pos;
    }
}
