using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class turretBullet : MonoBehaviour
{
    public Vector3 toVector { get; protected set; }
    protected float speed;
    public float damage { get; protected set; }
    protected float orbitDeleay = 0.01f;
    protected float targetFigure;
    private OrbitColors orbitColor;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,3f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(toVector * (speed * Time.deltaTime), Space.World);  
    }

    private bool isActive= true;
    private void OnDestroy()
    {
        isActive = false;
    }

    public void Init(GetFireInstance getinfo)
    {
        transform.position = getinfo.firepos;
        toVector = CustomAngle.VectorRotation(CustomAngle.PointDirection(getinfo.firepos,
            getinfo.mousepos));
        damage = getinfo.damage;
        speed = getinfo.speed;
        orbitColor = getinfo.orbitcolors;
        targetFigure = getinfo.targetFigure;
        transform.rotation = Quaternion.Euler(0, 0,
            CustomAngle.PointDirection(getinfo.firepos, getinfo.mousepos));
        MakeOrbit().Forget();
    }
    async UniTaskVoid MakeOrbit()
    {
        float z = 0;
        while (isActive)
        {
            GameManager.Instance._poolingManager.Spawn<Orbit>().Init(new OrbitInfo(true,transform.position
                +new Vector3(Random.Range(0.15f,-0.15f),Random.Range(0.15f,-0.15f),z+=0.01f),transform.localScale.x*0.5f,targetFigure,orbitColor),0.45f);
            await UniTask.Delay(TimeSpan.FromSeconds(orbitDeleay), ignoreTimeScale: false);
        }
    }
}
