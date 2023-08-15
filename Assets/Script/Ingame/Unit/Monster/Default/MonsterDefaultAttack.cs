using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MonsterDefaultAttack : MonoBehaviour
{
    [SerializeField] private Transform Weapon;
    protected MonsterDefault _monsterDefault;
    [SerializeField] private MonsterBullet _bullet;
    [SerializeField] private float _attackDelay;
    [SerializeField] private Transform _monsterGun;


    protected float _rotateDegree;
    // Start is called before the first frame update
    void Start()
    {
        _monsterDefault = gameObject.GetComponent<MonsterDefault>();
        
    }
    // Update is called once per frame
    protected void Update()
    {
        focusGun();
    }

    protected float toDegree;
    protected virtual void focusGun()
    {
        if (_monsterDefault._targetedPlayer)
        {
            
            _rotateDegree = CustomAngle.PointDirection(transform.position, _monsterDefault.player.transform.position);
            toDegree = _monsterGun.localEulerAngles.z;
            if (Mathf.Abs(toDegree-_rotateDegree)>180f) toDegree = _rotateDegree;
            toDegree += (_rotateDegree - toDegree) / 7.5f;
            _monsterGun.localEulerAngles = new Vector3(0, 0, toDegree);
            
            var localScale = _monsterGun.localScale;
            if ((toDegree is > 90f and < 270f) && localScale.y > 0)
                localScale = new Vector3(localScale.x, -localScale.y, 0);
            else if((toDegree is <= 90f or >= 270f) && localScale.y < 0)
                localScale = new Vector3(localScale.x, localScale.y, 0);

            _monsterGun.localScale += (localScale-_monsterGun.localScale)/7.5f;
        }
    }
    protected virtual async UniTaskVoid Attack()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_attackDelay), ignoreTimeScale: false);
        while (!_monsterDefault.isDie&&_monsterDefault._targetedPlayer)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_attackDelay), ignoreTimeScale: false);
        }
    }
    
    public void startAttack()
    {
        Attack().Forget();
    }
}
