using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using Cysharp.Threading;
using Unity.VisualScripting;

public class Bullet : PoolableObj
{
    public Vector3 toVector { get; protected set; }

    protected float speed;
    public float damage { get; protected set; }

    protected float orbitDeleay = 0.004f;
    protected float targetFigure;
    protected SpriteRenderer _sprite;

    public OrbitColors orbitColor { get; protected set; }

    // Start is called before the first frame update
    protected new void Start()
    {
        //base.Start();
        _sprite = GetComponent<SpriteRenderer>();
    }

    protected new void OnEnable()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    protected void Update()
    {
        transform.Translate(toVector * (speed * Time.deltaTime), Space.World);
    }

    private void FixedUpdate()
    {
    }

    private float time;

    protected void OnDisable()
    {
    }

    public virtual GameObject Init(GetFireInstance getinfo)
    {
       
        time = 0;
        transform.position = getinfo.firepos;
        toVector = CustomAngle.VectorRotation(CustomAngle.PointDirection(getinfo.playerpos,
            getinfo.mousepos) + Random.Range(getinfo.spread, -getinfo.spread));
        damage = getinfo.damage;
        speed = getinfo.speed;
        _sprite.color = getinfo.bulletColor;
        orbitColor = getinfo.orbitcolors;
        targetFigure = getinfo.targetFigure;
        transform.rotation = Quaternion.Euler(0, 0,
            CustomAngle.PointDirection(getinfo.firepos, getinfo.mousepos));

        MakeOrbit().Forget();
        ReleseReserv().Forget();
        return gameObject;
    }

    public GameObject Init(GetFireInstance getinfo, float angle)
    {
        transform.position = getinfo.firepos;
        toVector = CustomAngle.VectorRotation(CustomAngle.PointDirection(getinfo.playerpos,
            getinfo.mousepos) + angle + Random.Range(getinfo.spread, -getinfo.spread));
        damage = getinfo.damage;
        speed = getinfo.speed;
        _sprite.color = getinfo.bulletColor;
        orbitColor = getinfo.orbitcolors;
        targetFigure = getinfo.targetFigure;
        transform.rotation = Quaternion.Euler(0, 0,
            CustomAngle.PointDirection(getinfo.firepos, getinfo.mousepos));

        MakeOrbit().Forget();
        ReleseReserv().Forget();
        return gameObject;
    }

    protected override async UniTaskVoid ReleseReserv(float delay = 2)
    {
        while(gameObject.activeSelf)
        {
            delay -= 0.1f;
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), ignoreTimeScale: false);
            if (delay <= 0)
                break;
        }
        GameManager.Instance._poolingManager.Despawn(this);
    }


async UniTaskVoid MakeOrbit()
    {
        float z = 0;
        while (gameObject.activeSelf)
        {
            GameManager.Instance._poolingManager.Spawn<Orbit>().Init(new OrbitInfo(true,transform.position
            +new Vector3(Random.Range(0.15f,-0.15f),Random.Range(0.15f,-0.15f),z+=0.01f),transform.localScale.x,targetFigure,orbitColor),0.45f);
            await UniTask.Delay(TimeSpan.FromSeconds(orbitDeleay), ignoreTimeScale: false);
        }
    }
}


