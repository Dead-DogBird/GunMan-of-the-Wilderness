using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterBullet : PoolableObj
{
    public Vector3 toVector { get; private set;}
    private float speed;
    public float damage { get; private set; }
    private Color color;
    private Color priColor;
    private Color sceColor;
    private float orbitDeleay =0.004f;
    private SpriteRenderer _sprite;
    public OrbitColors orbitColor{ get; private set; }
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
    public GameObject Init(GetFireInstance getinfo)
    {
        transform.position = getinfo.firepos;
        toVector = CustomAngle.VectorRotation(CustomAngle.PointDirection(getinfo.playerpos,
            getinfo.mousepos)+getinfo.spread);
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
        if(gameObject.activeSelf)
            GameManager.Instance._poolingManager.Despawn(this); 
    }

    async UniTaskVoid MakeOrbit()
    {
        float z = 0;
        while (gameObject.activeSelf)
        {
            GameManager.Instance._poolingManager.Spawn<Orbit>().Init(new OrbitInfo(true,transform.position
                +new Vector3(Random.Range(0.15f,-0.15f),Random.Range(0.15f,-0.15f),z+=0.01f),transform.localScale.x,0.1f,orbitColor),0.45f);
            await UniTask.Delay(TimeSpan.FromSeconds(orbitDeleay), ignoreTimeScale: false);
        }
    }
}
