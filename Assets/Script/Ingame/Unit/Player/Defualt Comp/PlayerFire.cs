using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

//총알발사 담당
//이후의 프리팹은 이 스크립트를 상속해서 사용.
public class PlayerFire : MonoBehaviour
{
    protected PlayerState _playerState;
    protected float _myDamage { get; private set; }
    [SerializeField] protected GameObject _bullet;
    [SerializeField] protected GameObject _fireFlame;
    protected bool isDead = false;
    protected void Start()
    {
        _playerState = GetComponent<PlayerState>();
        _myDamage = _playerState.getDamage;
        GameManager.Instance._poolingManager.
            AddPoolingList<Bullet>(10,_bullet);
    }

    protected void OnDestroy()
    {
        isDead = true;
    }

    protected void Update()
    {
        if (_playerState.GetFire())Fire();
        if(_playerState.GetSkill())Skill();
    }
    protected virtual void Fire()
    {
        GameManager.Instance._poolingManager.Spawn<Bullet>().Init( _playerState.GetFireInstance());
        Instantiate(_fireFlame, _playerState.GetFireInstance().firepos,Quaternion.identity);
        GameManager.Instance.MoveOrbitEffect(_playerState.GetFireInstance().firepos,Random.Range(3,5),0.7f,
            _playerState.colors,
            false,0,2, (_playerState._playerGun.rotateDegree+180),Random.Range(7,12),30);

        
        _playerState.GetFireEffect();

    }
    protected virtual void Skill()
    {
    }
}
