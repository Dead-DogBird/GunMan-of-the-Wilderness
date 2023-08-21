using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DMRFire : PlayerFire
{
    [SerializeField] private GameObject Turret;
    [SerializeField] private float turretLenth = 5;
    public bool isTurretActive;
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
        _audioManager.PlaySFX(skillSfxId);
        var turret = Instantiate(Turret, transform.position, quaternion.identity);
            turret.GetComponent<DMRTurret>().
            Init(_playerState,this);
            Destroy(turret,turretLenth);
    }
}
