using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossMonster1 : DefaultBossMonster
{
    [SerializeField] private Transform firepos1, firepos2;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        BossPatterns = new BossPattern[]{BossPattern1,BossPattern2,BossPattern3,BossPattern4};
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        MoveToPlayer();
    }

    //탄막발사
    async UniTaskVoid BossPattern1()
    {
        isBossPattern = true;
        for (int i = 0; i < 4; i++)
        {
            for (int j = -4; j < 3; j++)
            {
                AudioManager.Instance.PlaySFX(17, false, 1);

                GameManager.Instance._poolingManager.Spawn<MonsterBullet>().Init(new GetFireInstance(transform.position,
                    firepos1.position, _player.transform.position,
                    15,10, BulletColor, new OrbitColors(priColor, secColor),0.1f,30*j));
                GameManager.Instance._poolingManager.Spawn<MonsterBullet>().Init(new GetFireInstance(transform.position,
                    firepos2.position, new Vector3(-_player.transform.position.x,_player.transform.position.y),
                    15,10, BulletColor, new OrbitColors(priColor, secColor),0.1f,30*j));
                if (isDie) return;
            }
            if (isDie) return;
            for (int k = 10; k >= 0; k--)
            {
                if (isDie) return;
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            }
        }
        for (int i = 30; i >= 0; i--)
        {
            if (isDie) return;
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        }
        isBossPattern = false;
    }
    //총알 비
    async UniTaskVoid BossPattern2()
    {
        isBossPattern = true;
        for (int i = 0; i < 30; i++)
        {
            float randompos = Random.Range(-10.0f, 10.0f);
            GameManager.Instance._poolingManager.Spawn<MonsterBullet>().Init(new GetFireInstance(transform.position+new Vector3(randompos,10),
                transform.position+new Vector3(randompos,10), new Vector3( transform.position.x+Random.Range(-10.0f, 10.0f),0),
                    15,10, BulletColor, new OrbitColors(priColor, secColor)));
                if (isDie) return;
            for (int k = 3; k >= 0; k--)
            {
                if (isDie) return;
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            }
        }
        for (int i = 30; i >= 0; i--)
        {
            if (isDie) return;
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        }
        isBossPattern = false;
    }
    //돌진
    async UniTaskVoid BossPattern3()
    {
        float oriSpeed = _speed;
        isBossPattern = true;
        _speed =0;
        isCharge = true;
        
        for(int i =0;i<3;i++)
        {    
            float Tospeed = 1.2f*8* (_player.transform.position.x > transform.position.x ? 2 : -2);
            if (_player.transform.position.x < transform.position.x&&transform.localScale.x<0)
            {
                transform.localScale = new Vector3(1.5f, 1.5f, 1);
            }
            else if (_player.transform.position.x > transform.position.x&&transform.localScale.x>0)
            {
                transform.localScale = new Vector3(-1.5f, 1.5f, 1);
            }
            if (isDie) return;
            float timer = 1f;
            while(timer>0)
            {
                timer -= Time.deltaTime;
                transform.Translate(new Vector3(Tospeed, 0) * Time.deltaTime);
                await UniTask.Yield(PlayerLoopTiming.LastUpdate);
            }
            _speed = oriSpeed;
            for (int k = 7; k >= 0; k--)
            {
                if (isDie) return;
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            }
        }
        isCharge = false;
        _speed = oriSpeed;
        for (int i = 15; i >= 0; i--)
        {
            if (isDie) return;
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        }
        isBossPattern = false;
    }
    //그냥 총 발사
    async UniTaskVoid BossPattern4()
    {
        isBossPattern = true;
        for (int j = 0; j <= 5; j++)
        {
            AudioManager.Instance.PlaySFX(17, false, 1);
            GameManager.Instance._poolingManager.Spawn<MonsterBullet>().Init(new GetFireInstance(transform.position,
                firepos1.position, _player.transform.position,
                20,12, BulletColor, new OrbitColors(priColor, secColor),0.1f,Random.Range(-10.0f,10.0f)));
            GameManager.Instance._poolingManager.Spawn<MonsterBullet>().Init(new GetFireInstance(transform.position,
                firepos2.position, _player.transform.position,
                20,12, BulletColor, new OrbitColors(priColor, secColor),0.1f,Random.Range(-10.0f,10.0f)));
            if (isDie) return;
            for (int k = 7; k >= 0; k--)
            {
                if (isDie) return;
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            }
        }
        for (int i = 10; i >= 0; i--)
        {
            if (isDie) return;
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        }
        isBossPattern = false;
    }

    private bool isCharge = false;
    void MoveToPlayer()
    {
        if (isCharge) return;
        transform.Translate(new Vector3(_speed*(_player.transform.position.x > transform.position.x? 2 : -2), 0) * Time.deltaTime);
        if (_player.transform.position.x < transform.position.x&&transform.localScale.x<0)
        {
            transform.localScale = new Vector3(1.5f, 1.5f, 1);
        }
        else if (_player.transform.position.x > transform.position.x&&transform.localScale.x>0)
        {
            transform.localScale = new Vector3(-1.5f, 1.5f, 1);
        }
    }
    
}
