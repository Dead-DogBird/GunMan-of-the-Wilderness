using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGunFire : PlayerFire
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

    protected override void Fire()
    {
        for (int i = 2; i > -3; i--)
        {
            GameManager.Instance._poolingManager.Spawn<Bullet>().Init(_playerState.GetFireInstance(),i*10);
        }

        Instantiate(_fireFlame, _playerState.GetFireInstance().firepos, Quaternion.identity).transform
            .localEulerAngles = new Vector3(0, 0, CustomAngle.PointDirection(_playerState.GetFireInstance().firepos,
            _playerState.GetFireInstance().mousepos));
        _playerState.GetFireEffect();
    }
}

