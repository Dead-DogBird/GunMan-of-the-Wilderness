using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MonsterDefaultAttack : MonoBehaviour
{
    protected MonsterDefault _monsterDefault;
    [SerializeField] protected MonsterBullet _bullet;
    [SerializeField] protected float _attackDelay;
    [SerializeField] protected Transform _monsterGun;
    [SerializeField] protected GameObject _fireflame;
    [SerializeField] protected Transform firepos;
    [SerializeField] protected Color bulletColor,pricolor,seccolor;
    public Color PriColor => pricolor;
    protected float _rotateDegree;
    protected float toDegree;
    protected Vector3 oriscale;
    public float damage;
    // Start is called before the first frame update
    protected void Start()
    {
        oriscale = _monsterGun.localScale;
        _monsterDefault = gameObject.GetComponent<MonsterDefault>();
        GameManager.Instance._poolingManager.
            AddPoolingList<MonsterBullet>(10,_bullet.gameObject);
    }
    // Update is called once per frame
    protected void Update()
    {
        focusGun();
    }


    protected virtual void focusGun()
    {
        if (_monsterDefault._targetedPlayer)
        {
            var target = _monsterDefault.player.transform.position - transform.position;
            _rotateDegree = -1 * Mathf.Atan2(target.x, target.y) * Mathf.Rad2Deg + 90;
            if(transform.localScale.x < 0) _rotateDegree += 180;
            _monsterGun.rotation = Quaternion.Euler(0,0,_rotateDegree);
            //_rotateDegree = CustomAngle.PointDirection(transform.position, _monsterDefault.player.transform.position);
        }
    }
    protected virtual async UniTaskVoid Attack()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_attackDelay), ignoreTimeScale: false);
        while (!_monsterDefault.isDie&&_monsterDefault._targetedPlayer)
        {
            GameManager.Instance._poolingManager.Spawn<MonsterBullet>().Init( new GetFireInstance(transform.position,firepos.position,_monsterDefault.player.transform.position,
               damage,_monsterDefault.bulletSpeed,bulletColor,new OrbitColors(pricolor,seccolor)));
            Instantiate(_fireflame,firepos.position,Quaternion.identity);
            await UniTask.Delay(TimeSpan.FromSeconds(_attackDelay), ignoreTimeScale: false);
        }
    }
    
    public void startAttack()
    {
        Attack().Forget();
    }
}
