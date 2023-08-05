using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//총알발사 담당
//이후의 프리팹은 이 스크립트를 상속해서 사용.
public class PlayerFire : MonoBehaviour
{
    private PlayerState _playerState;
    protected float _myDamage { get; private set; }
    [SerializeField] private GameObject _bullet;
    [SerializeField] private Transform _gunholePos;
    void Start()
    {
        _playerState = GetComponent<PlayerState>();
        _myDamage = _playerState.getDamage;
        GameManager.Instance._poolingManager.
            AddPoolingList<Bullet>(10,_bullet);
    }

    void Update()
    {
        if (_playerState.GetFire())Fire();
        if(_playerState.GetSkill())Skill();
    }
    protected virtual void Fire()
    {
        Debug.Log("발사!");
        GameManager.Instance._poolingManager.Spawn<Bullet>().Init(_gunholePos.position,
            _playerState.GetMousePos(), 10, _playerState.getBulletSpeed,
            Color.black, new OrbitColors(Color.black, Color.black));
    }
    protected virtual void Skill()
    {
        //Debug.Log("기술발동");
    }
}
