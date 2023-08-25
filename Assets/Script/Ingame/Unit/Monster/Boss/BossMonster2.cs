using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossMonster2 : DefaultBossMonster
{
    [SerializeField] private Transform firepos;
    [SerializeField] private Transform Gun;
    [SerializeField] private GameObject[] Monsters;
    [SerializeField] private GameObject Raser;
    private List<GameObject> MonstersList;
    // Start is called before the first frame update
    void Start()
    {
        MonstersList = new List<GameObject>();
        base.Start();
        BossPatterns = new BossPattern[]{BossPattern1,BossPattern2,BossPattern3};
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        MoveToPlayer();
        focusGun();
    }

    protected override async UniTaskVoid DieTask()
    {
        isDie = true;
        for (int i = 0; i < MonstersList.Count; i++)
        {
            Destroy(MonstersList.First());
            MonstersList.RemoveAt(0);
        }
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
        for (int i = 0; i < 10; i++)
        {
            GameManager.Instance.SpawnCoin(transform.position+new Vector3(Random.Range(-3.0f,3.0f),0), 350);
        }
        instGate = Instantiate(Gate,new Vector3(75,0), quaternion.identity);
        UIManager.Instance.SetBossUI();
    }
    async UniTaskVoid BossPattern1()
    {
        isBossPattern = true;
        for (int j = 0; j <= 10; j++)
        {
            AudioManager.Instance.PlaySFX(17, false, 1);
            GameManager.Instance._poolingManager.Spawn<MonsterBullet>().Init(new GetFireInstance(transform.position,
                firepos.position, _player.transform.position,
                25,12, BulletColor, new OrbitColors(priColor, secColor),0.1f,Random.Range(-10.0f,10.0f)));
             if (isDie) return;
            for (int k = 5; k >= 0; k--)
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
    async UniTaskVoid BossPattern2()
    {
        isBossPattern = true;
        for (int i = 0; i < 5; i++)
        {
            Instantiate(Monsters[Random.Range(0, Monsters.Length)]).transform.position =
                new Vector3(transform.position.x+Random.Range(-3.0f, 3.0f), 10);
            for (int k = 2; k >= 0; k--)
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
    async UniTaskVoid BossPattern3()
    {
        isBossPattern = true;
        GameObject temp;
        for (int i = 0; i < 5; i++)
        {
            AudioManager.Instance.PlaySfXDelayed(13,0.3f,false,1,0.5f);
            temp = Instantiate(Raser);
            temp.transform.position = new Vector3(transform.position.x+Random.Range(-10.0f, 10.0f), Random.Range(7.0f,8.5f));
            Destroy(temp.gameObject,2);
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
