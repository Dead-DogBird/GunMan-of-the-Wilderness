using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;


public class SniperFire : PlayerFire
{
    private Player_Gun _playerGun;

    private bool isUlt = false;
    public float skilltime = 7;
    private int ultBullet = 5;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        _playerGun = GetComponent<Player_Gun>();
    }
    
    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (isUlt)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Time.timeScale = 0.2f;
            }
            if (Input.GetMouseButtonUp(1))
            {
                Time.timeScale = 1;
            }
        }
    }
    protected override void Fire()
    {
        if (isUlt)
        {
            ultBullet--;
            if (ultBullet == 0) 
                Time.timeScale = 1;
        
        }
        IngameCamera.Instance.Shake(0.15f,0,0,1,6f);
        GameManager.Instance._poolingManager.Spawn<Bullet>().Init( _playerState.GetFireInstance());
        Instantiate(_fireFlame, _playerState.GetFireInstance().firepos, Quaternion.identity).
            transform.localEulerAngles = new Vector3(0,0,_playerGun.rotateDegree);
        _playerState.GetFireEffect();
        
    }

    protected override void Skill()
    {
        ultBullet = 5;
        SkillTask().Forget();
    }

    async UniTaskVoid SkillTask()
    {
        float time = skilltime;
        isUlt = true;
        _playerState.SniperUlt(isUlt);
        while (ultBullet > 0&&time>0)
        {
            time -= Time.unscaledDeltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        isUlt = false;
        _playerState.SniperUlt(isUlt);
        Time.timeScale = 1;
    }

}
