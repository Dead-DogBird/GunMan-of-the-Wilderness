using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
using Range = UnityEngine.SocialPlatforms.Range;


public class SniperFire : PlayerFire
{

    private bool isUlt = false;
    public float skilltime = 7;
    private int ultBullet = 5;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        
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
                GameManager.Instance.SniperSkill(true);
            }
            if (Input.GetMouseButtonUp(1))
            {
                Time.timeScale = 1;
                GameManager.Instance.SniperSkill(false);
            }
        }
    }
    protected override void Fire()
    {
        UIManager.Instance.SetCursorEffect(1.5f);
        if (isUlt)
        {
            ultBullet--;
            UIManager.Instance.SniperSkillFire();
            if (ultBullet == 0)
            {
                Time.timeScale = 1;
                GameManager.Instance.SniperSkill(false);
            }
        
        }
        IngameCamera.Instance.Shake(0.15f,0,0,1,6f);
        GameManager.Instance._poolingManager.Spawn<Bullet>().Init( _playerState.GetFireInstance());
      
        Instantiate(_fireFlame, _playerState.GetFireInstance().firepos, Quaternion.identity).
            transform.localEulerAngles = new Vector3(0,0,_playerState._playerGun.rotateDegree);
       
        GameManager.Instance.MoveOrbitEffect(_playerState.GetFireInstance().firepos,Random.Range(6,7),0.9f,
           new OrbitColors(_playerState.colors.priColor,_playerState.colors.secColor),
            false,0,2, (_playerState._playerGun.rotateDegree+180),Random.Range(7,12),30);

        _playerState.GetFireEffect();
        
    }

    protected override void Skill()
    {
        _audioManager.PlaySFX(skillSfxId);
        _playerState.SetMaxMag();
        ultBullet = _playerState.getAllMag;
        IngameCamera.Instance.Shake(0,0.2f,0,1,6f);
        SkillTask().Forget();
    }
    float time;
    async UniTaskVoid SkillTask()
    {
        time = skilltime;
        isUlt = true;
        _playerState.SniperUlt(isUlt);
        OnskillEffect().Forget();
        while ((ultBullet > 0&&time>0)&&!isDead)
        {
            
            time -= Time.unscaledDeltaTime;
            UIManager.Instance.UpdateSniperSkillGauge(time / skilltime);
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        isUlt = false;
        GameManager.Instance.SniperSkill(isUlt);
        _playerState.SniperUlt(isUlt);
        _playerState.SetMaxMag();
        UIManager.Instance.UpdateSniperSkillGauge(0);
        Time.timeScale = 1;
    }

    async UniTaskVoid OnskillEffect()
    {
        while ((ultBullet > 0&&time>0)&&!isDead)
        {
            float randomAngle = Random.Range(0.0f, 360.0f);
            float radianAngle = randomAngle * Mathf.Deg2Rad;
            float x = 0.7f * Mathf.Cos(radianAngle);
            float y = 0.7f * Mathf.Sin(radianAngle);
            
            float radom = Random.Range(90f/255f, 235f/255f);
            GameManager.Instance.MoveOrbitEffect(transform.position+new Vector3(x, y+0.5f),1,0.7f,
            new OrbitColors(new Color(radom-100/255f,radom-100/255f,255/255f,0.2f),new Color(radom,radom,255/255f,1f)),
            false,0,2,90,Random.Range(7,12),15);
            await UniTask.Delay(TimeSpan.FromSeconds(0.01f),true);
        }
    }

}
