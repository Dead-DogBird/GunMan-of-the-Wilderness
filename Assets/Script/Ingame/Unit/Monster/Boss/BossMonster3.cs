using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossMonster3 : DefaultBossMonster
{
    [SerializeField] private Transform firepos;
    [SerializeField] private Transform Gun;
    [SerializeField] private GameObject Explosion;
    [SerializeField] private GameObject Raser;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        BossPatterns = new BossPattern[]{BossPattern1,BossPattern2,BossPattern3,BossPattern4,BossPattern5};
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        MoveToPlayer();
        focusGun();
    }
    //총알 발사
    async UniTaskVoid BossPattern1()
    {
        isBossPattern = true;
        _speed *= 0.2f;
        for (int i = 0; i < 4; i++)
        {
            for (int j = -4; j < 3; j++)
            {
                AudioManager.Instance.PlaySFX(17, false, 1);

                GameManager.Instance._poolingManager.Spawn<MonsterBullet>().Init(new GetFireInstance(transform.position,
                    firepos.position, _player.transform.position,
                    15,12, BulletColor, new OrbitColors(priColor, secColor),0.1f,30*j));
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
        _speed *= 5f;
        isBossPattern = false;
    }
    //폭발 생성
    async UniTaskVoid BossPattern2()
    {
        isBossPattern = true;
        for (int i = 0; i < 10; i++)
        {
            if (isDie) return;
            Instantiate(Explosion).transform.position = new Vector3(transform.position.x + Random.Range(-18.5f,18.5f),transform.position.y + Random.Range(-2.5f,9.5f));
        }
        for (int i = 30; i >= 0; i--)
        {
            if (isDie) return;
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        }
        isBossPattern = false;
    }
    //유저 위로 텔레포트
    async UniTaskVoid BossPattern3()
    {
        isBossPattern = true;
        float orispeed = _speed;
        for (int i = 0; i < 30; i++)
        {
            if (isDie) return;
            float angle = Random.Range(0f, 360f); // 0부터 360도 사이의 랜덤한 각도
            float radians = angle * Mathf.Deg2Rad; // 각도를 라디안으로 변환
            float x = 2f * Mathf.Cos(radians); // x 좌표 계산
            float y = 2f * Mathf.Sin(radians);
            GameManager.Instance.MoveOrbitEffect(transform.position+new Vector3(x,y),1,0.7f,
                colors,false,0,2,CustomAngle.PointDirection(transform.position,transform.position+new Vector3(x,y)));
        }
        transform.localScale = Vector3.zero;
        _speed = 0;
        for (int i = 15; i >= 0; i--)
        {
            if (isDie) return;
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        }
        for (int i = 0; i < 30; i++)
        {
            if (isDie) return;
            float angle = Random.Range(0f, 360f); // 0부터 360도 사이의 랜덤한 각도
            float radians = angle * Mathf.Deg2Rad; // 각도를 라디안으로 변환
            float x = 2f * Mathf.Cos(radians); // x 좌표 계산
            float y = 2f * Mathf.Sin(radians);
            GameManager.Instance.MoveOrbitEffect(transform.position+new Vector3(x,y),1,0.7f,
                colors,false,0,2,CustomAngle.PointDirection(transform.position,transform.position+new Vector3(x,y))-180);
        }
        transform.localScale = Vector3.one;
        _rigid.velocity = Vector2.zero;
        _speed = orispeed;
        transform.position = new Vector3(_player.transform.position.x+2, _player.transform.position.y + 2);
        for (int i = 10; i >= 0; i--)
        {
            if (isDie) return;
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        }
        isBossPattern = false;
    }
    //레이저 발사
    async UniTaskVoid BossPattern4()
    {
        isBossPattern = true;
        GameObject temp;
        for (int i = 0; i < 5; i++)
        {
            AudioManager.Instance.PlaySfXDelayed(13,0.3f,false,1,0.5f);
            temp = Instantiate(Raser);
            temp.transform.position = new Vector3(transform.position.x+Random.Range(-15.0f, 15.0f), Random.Range(7.0f,8.5f));
            Destroy(temp.gameObject,2);
            for (int k = 7; k >= 0; k--)
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
    async UniTaskVoid BossPattern5()
    {
        isBossPattern = true;
        float orispeed = _speed;
        _speed = 0;
        for (int j = 0; j <= 15; j++)
        {
            AudioManager.Instance.PlaySFX(2, false, 1);
            GameManager.Instance._poolingManager.Spawn<MonsterBullet>().Init(new GetFireInstance(transform.position,
                firepos.position, _player.transform.position,
                25,15, BulletColor, new OrbitColors(priColor, secColor),0.1f,Random.Range(-10.0f,10.0f)));
            if (isDie) return;
            for (int k = 2; k >= 0; k--)
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
        _speed = orispeed;
        isBossPattern = false;
    }
    protected float _rotateDegree;
    void focusGun()
    {
        var target = _player.transform.position - transform.position;
        _rotateDegree = -1 * Mathf.Atan2(target.x, target.y) * Mathf.Rad2Deg + 90;
        if(transform.localScale.x < 0) _rotateDegree += 180;
        Gun.rotation = Quaternion.Euler(0,0,_rotateDegree);
    }
    void MoveToPlayer()
    {
        if (Mathf.Abs(transform.position.x - _player.transform.position.x) >= 3)
        {
            transform.Translate(new Vector3(_speed*(_player.transform.position.x > transform.position.x? 2 : -2), 0) * Time.deltaTime);
        }
        else
        {
            transform.Translate(new Vector3(_speed*(_player.transform.position.x > transform.position.x? -1.5f : 1.5f), 0) * Time.deltaTime);
        }
        if (_player.transform.position.x < transform.position.x&&transform.localScale.x<0)
        {
            transform.localScale = new Vector3(1.5f, 1.5f, 1);
        }
        else if (_player.transform.position.x > transform.position.x&&transform.localScale.x>0)
        {
            transform.localScale = new Vector3(-1.5f, 1.5f, 1);
        }
    }
    protected override async UniTaskVoid DieTask()
    {
        isDie = true;
        Time.timeScale = 0.3f;
        IngameCamera.Instance.Shake(0.05f,0.05f,0,1,60);
        await UniTask.Delay(TimeSpan.FromSeconds(0.4f),true);
        AudioManager.Instance.PlaySFX(20);
        for (int i = 0; i < 100; i++)
        {
            GameManager.Instance.MoveOrbitEffect(transform.position+new Vector3(0,0.5f),Random.Range(3,5),1.5f,
                colors, false,0,2, 180,Random.Range(7,12),180);
            await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime), true);
        }
        IngameCamera.Instance.Shake(0.3f,0.3f,0,1,20);
        transform.localScale = Vector3.zero;
        Time.timeScale = 1f;
        UIManager.Instance.SetBossUI();
        await UniTask.Delay(TimeSpan.FromSeconds(3), true);
        UIManager.Instance.SetFade(true, 0).Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(1), true);
        GameManager.Instance.GetEnding();
    }
}
