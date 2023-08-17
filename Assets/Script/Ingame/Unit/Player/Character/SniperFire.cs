using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public class SniperFire : PlayerFire
{
    private Player_Gun _playerGun;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        _playerGun = GetComponent<Player_Gun>();
    }
    
    // Update is called once per frame
    void Update()
    {
        base.Update();
    }
    protected override void Fire()
    {
        IngameCamera.Instance.Shake(0.15f,0,0,1,6f);
        GameManager.Instance._poolingManager.Spawn<Bullet>().Init( _playerState.GetFireInstance());
        Instantiate(_fireFlame, _playerState.GetFireInstance().firepos, Quaternion.identity).
            transform.localEulerAngles = new Vector3(0,0,_playerGun.rotateDegree);
        _playerState.GetFireEffect();
        
    }



}
