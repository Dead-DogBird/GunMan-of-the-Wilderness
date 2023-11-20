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
                AudioManager.Instance.SetMusicPitch(0.2f,true);
            }
            if (Input.GetMouseButtonUp(1))
            {
                Time.timeScale = 1;
                AudioManager.Instance.SetMusicPitch(1,false);
                GameManager.Instance.SniperSkill(false);
            }
        }
    }
    protected override void Fire()
    {
        _audioManager.PlaySFX(fireSfxId);
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
        ultBullet = _playerState.getAllMag;
        IngameCamera.Instance.Shake(0,0.2f,0,1,6f);
        SkillTask().Forget();
    }
    float time;
    async UniTaskVoid SkillTask()
    {
        skilltime = _playerState.getAllMag+2.5f;
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
        UIManager.Instance.UpdateSniperSkillGauge(0);
        AudioManager.Instance.SetMusicPitch(1);
        Time.timeScale = 1;
    }

    async UniTaskVoid OnskillEffect()
    {
        while ((ultBullet > 0&&time>0)&&!isDead)
        {
            float angle = Random.Range(0f, 360f); // 0부터 360도 사이의 랜덤한 각도
            float radians = angle * Mathf.Deg2Rad; // 각도를 라디안으로 변환
            float x = 2f * Mathf.Cos(radians); // x 좌표 계산
            float y = 2f * Mathf.Sin(radians);
            GameManager.Instance.MoveOrbitEffect(transform.position+new Vector3(x-0.3f,y+0.7f),Random.Range(3,5),0.7f,
                new OrbitColors(Color.white, _playerState.colors.priColor), false,0,2,CustomAngle.PointDirection(transform.position,transform.position+new Vector3(x-0.3f,y+0.7f))-150,Random.Range(7,12),0);

            await UniTask.Delay(TimeSpan.FromSeconds(0.01f),true);
        }
    }

}
