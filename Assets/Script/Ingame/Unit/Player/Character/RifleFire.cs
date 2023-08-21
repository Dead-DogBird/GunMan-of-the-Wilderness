using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class RifleFire : PlayerFire
{
    [SerializeField] private GameObject _raser;
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
    protected override void Skill()
    {
        _audioManager.PlaySfXDelayed(skillSfxId,0.3f,false,1,1);
        var obj = Instantiate(_raser, _playerState.GetFireInstance().mousepos +
                                      new Vector3(0, 6.5f, 10), quaternion.identity);
        Destroy(obj,2);
    }
}
