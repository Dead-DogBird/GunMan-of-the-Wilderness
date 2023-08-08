using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : PoolableObj
{
    private Vector3 toVector;

    private float speed;
    private float damage;

    private Color color;

    private Color priColor;
    private Color sceColor;

    private float orbitDeleay =0.004f;

    private SpriteRenderer _sprite;
    private OrbitColors orbitColor;
    // Start is called before the first frame update
    private new void Start()
    {
        //base.Start();
        _sprite = GetComponent<SpriteRenderer>();
    }

    new void OnEnable()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(toVector * (speed * Time.deltaTime),Space.World);
    }

    private void FixedUpdate()
    {
        if (damage - 0.5f > 5)
            damage -= 0.5f;
    }

    public GameObject Init(GetFireInstance getinfo)
    {
        transform.position = getinfo.firepos;
        toVector = CustomAngle.VectorRotation(CustomAngle.PointDirection(getinfo.firepos,
            getinfo.mousepos));
        damage = getinfo.damage;
        speed = getinfo.speed;
        _sprite.color = getinfo.bulletColor;
        orbitColor = getinfo.orbitcolors;
        transform.rotation = Quaternion.Euler(0,0,
            CustomAngle.PointDirection(getinfo.firepos,getinfo.mousepos));
        
        MakeOrbit().Forget();
        ReleseReserv().Forget();
        return gameObject;
    }

    protected override async UniTaskVoid ReleseReserv(float delay = 2)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false);
        GameManager.Instance._poolingManager.Despawn(this); 
    }

    async UniTaskVoid MakeOrbit()
    {
        while (gameObject.activeSelf)
        {
            GameManager.Instance._poolingManager.Spawn<Orbit>().Init(new OrbitInfo(true,transform.position
            +new Vector3(Random.Range(0.15f,-0.15f),Random.Range(0.15f,-0.15f),0.1f),transform.localScale.x,0.1f,orbitColor),0.45f);
            await UniTask.Delay(TimeSpan.FromSeconds(orbitDeleay), ignoreTimeScale: false);
        }
    }
}


