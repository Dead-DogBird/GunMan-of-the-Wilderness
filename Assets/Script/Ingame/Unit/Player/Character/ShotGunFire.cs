using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShotGunFire : PlayerFire
{
    [SerializeField] private GameObject ShotGunSkill;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    protected override void Fire()
    {
        UIManager.Instance.SetCursorEffect(1.5f);
        for (int i = 2; i > -3; i--)
        {
            GameManager.Instance._poolingManager.Spawn<Bullet>().Init(_playerState.GetFireInstance(),i*10);
        }

        Instantiate(_fireFlame, _playerState.GetFireInstance().firepos, Quaternion.identity).transform
            .localEulerAngles = new Vector3(0, 0, CustomAngle.PointDirection(_playerState.GetFireInstance().firepos,
            _playerState.GetFireInstance().mousepos));
        
        GameManager.Instance.MoveOrbitEffect(_playerState.GetFireInstance().firepos,Random.Range(3,5),0.7f,
            _playerState.colors,
            false,0,2, (_playerState._playerGun.rotateDegree+180) + Random.Range(-15f,15f),Random.Range(7,12));

        _playerState.GetFireEffect();
    }

    protected override void Skill()
    {
        SkillTask().Forget();
    }
    async UniTaskVoid SkillTask()
    {
        Vector3 pos = transform.position;
        _audioManager.PlaySFX(skillSfxId,false,1);
        for (int i = 1; i <= 4; i++)
        {
            IngameCamera.Instance.Shake(0, 0.1f, 0, 1, 20);
            RaycastHit2D ground = Physics2D.Raycast(pos + new Vector3(2.3f*i, 0), Vector2.down, 30,
                LayerMask.GetMask("Platform"));
            Instantiate(ShotGunSkill).transform.position =
                new Vector3(pos.x + (2.3f * i), ground.point.y);
            ground = Physics2D.Raycast(pos + new Vector3(-2.3f*i, 0), Vector2.down, 10,
                LayerMask.GetMask("Platform"));
            Instantiate(ShotGunSkill).transform.position =
                new Vector3(pos.x - (2.3f * i), ground.point.y);
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f),false);
        }
    }
}

