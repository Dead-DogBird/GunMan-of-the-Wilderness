using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShotGunMonsterAttack : MonsterDefaultAttack
{
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

    protected override async UniTaskVoid Attack()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_attackDelay), ignoreTimeScale: false);
        while (!_monsterDefault.isDie && _monsterDefault._targetedPlayer)
        {
            for (int i = 1; i > -2; i--)
            {
                GameManager.Instance._poolingManager.Spawn<MonsterBullet>().Init(new GetFireInstance(firepos.position,
                    _monsterDefault.player.transform.position,
                    _monsterDefault.damage, _monsterDefault.bulletSpeed, bulletColor,
                    new OrbitColors(pricolor, seccolor),10*i), transform.position);
            }
            Instantiate(_fireflame, firepos.position, Quaternion.identity);
            await UniTask.Delay(TimeSpan.FromSeconds(_attackDelay), ignoreTimeScale: false);
        }
    }
}
