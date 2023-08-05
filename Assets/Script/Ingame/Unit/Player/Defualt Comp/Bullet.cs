using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Bullet : PoolableObj
{
    private Vector3 toVector;

    private float speed;
    private float damage;

    private Color color;

    private Color priColor;
    private Color sceColor;

    private float orbitDeleay =0.004f;

    private OrbitColors orbitColor;
    // Start is called before the first frame update
    private new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (damage - 0.5f > 5)
            damage -= 0.5f;
        transform.Translate(toVector*speed,Space.World);
    }

    public GameObject Init(Vector3 pos, Vector3 pTovector,float pDamege,float pSpeed, Color pColor, OrbitColors pOrbitColors)
    {
        transform.position = pos;
        toVector = CustomAngle.VectorRotation(CustomAngle.PointDirection(pos, pTovector));
        damage = pDamege;
        speed = pSpeed;
        color = pColor;
        orbitColor = pOrbitColors;
        transform.rotation = Quaternion.Euler(0,0,
            CustomAngle.PointDirection(pos,pTovector));
        
        return gameObject;
    }

    protected override async UniTaskVoid ReleseReserv(float delay = 2)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false);
        GameManager.Instance._poolingManager.Despawn(this); 
    }
}

public struct OrbitColors
{
    public Color priColor;
    public Color sceColor;

    public OrbitColors(Color ppriColor,Color psceColor)
    {
        priColor = ppriColor;
        sceColor = psceColor;
    }
}
