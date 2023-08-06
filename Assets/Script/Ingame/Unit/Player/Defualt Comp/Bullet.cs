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

    public GameObject Init(GetFireInstance getinfo)
    {
        transform.position = getinfo.firepos;
        toVector = CustomAngle.VectorRotation(CustomAngle.PointDirection(getinfo.firepos,
            getinfo.mousepos));
        damage = getinfo.damage;
        speed = getinfo.speed;
        color = getinfo.bulletColor;
        orbitColor = getinfo.orbitcolors;
        transform.rotation = Quaternion.Euler(0,0,
            CustomAngle.PointDirection(getinfo.firepos,getinfo.mousepos));
        
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
