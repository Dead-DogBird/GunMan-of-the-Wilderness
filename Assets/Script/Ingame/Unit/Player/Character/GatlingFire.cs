using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GatlingFire : PlayerFire
{
    private int count;
    [SerializeField] private Transform[] firePoses;
    [SerializeField] private GameObject _skill;
    void Start()
    {
        base.Start();
        _skill.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }
    protected override void Skill()
    {
        SkillTask().Forget();
    }
    async UniTask SkillTask()
    {
        float time = 3;
        _audioManager.PlaySFX(skillSfxId);
        IngameCamera.Instance.Shake(0,0.3f,0,1,4f);
        _skill.SetActive(true);
        while (time > 0)
        {
            time -= 0.1f;
            float angle = Random.Range(0f, 360f); // 0부터 360도 사이의 랜덤한 각도
            float radians = angle * Mathf.Deg2Rad; // 각도를 라디안으로 변환
            float x = 3.25f * Mathf.Cos(radians); // x 좌표 계산
            float y = 3.25f * Mathf.Sin(radians);
            GameManager.Instance.MoveOrbitEffect(transform.position + new Vector3(x, y), Random.Range(3, 5), Random.Range(0.7f,2.0f), _playerState.colors,false,0,
            2,90,Random.Range(7,30),0);
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        }
        _skill.SetActive(false);
        IngameCamera.Instance.Shake(0,0.3f,0,1,4f);
    }
    protected override void Fire()
    {
        count++;
        var instance = _playerState.GetFireInstance();
        instance.firepos = firePoses[Random.Range(0, firePoses.Length)].position;
        if(count%4==0)
        {
            UIManager.Instance.SetCursorEffect();
            Instantiate(_fireFlame, instance.firepos,Quaternion.identity).
                transform.localEulerAngles = new Vector3(0,0,_playerState._playerGun.rotateDegree);
        }

        if (count % 3 == 0)
        {
           AudioManager.Instance.PlaySFX(26,false,0.2f);
        }
        _playerState.GetFireEffect();
        GameManager.Instance._poolingManager.Spawn<Bullet>().Init(instance,0.13f);
        GameManager.Instance.MoveOrbitEffect(instance.firepos,Random.Range(3,5),0.7f,
            _playerState.colors,
            false,0,2, (_playerState._playerGun.rotateDegree+180),Random.Range(7,12),30);
    }
}
