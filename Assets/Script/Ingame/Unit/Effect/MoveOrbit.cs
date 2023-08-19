using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mono.Cecil.Cil;
using UnityEngine;

public class MoveOrbit : Orbit
{
    private Vector3 _toVector;
    private float _speed;
    // Start is called before the first frame update
     void Start()
    {
        base.Start();    
    }

     void OnEnable()
     {
      base.OnEnable();
     }

     void OnDisable()
     {
         base.OnDisable();
     }
    // Update is called once per frame
    void Update()
    {
        transform.localScale -= new Vector3(Time.deltaTime*sizeReduction, Time.deltaTime*sizeReduction);
        //transform.localScale *= Time.deltaTime*sizeReduction;
        if (transform.localScale.x <= targetFigure)
        {
            if(colored)
                _tween.Complete();
            GameManager.Instance._poolingManager.Despawn(this);
        }

        transform.Translate(_toVector * (_speed * Time.deltaTime), Space.World);
    }

    public Orbit Init(OrbitInfo info, bool isSize = false, float reduction = 2,float angle = 0,float speed = 10)
    {
        var orbit = base.Init(info, isSize, reduction);
        _toVector = CustomAngle.VectorRotation(angle);
        _speed = speed;
        return orbit;
    }
    public void Init(OrbitInfo info, float outline, bool isSize = false, float reduction = 2,float angle = 0,float speed = 10)
    {
        var orbit = Init(info,isSize,reduction,angle,speed);
        GameManager.Instance._poolingManager.
            Spawn<MoveOrbit>().Init(new OrbitInfo(false,orbit.transform.position+new Vector3(0,0,0.1f),
                orbit.transform.localScale.x+outline,orbit.targetFigure+outline),isSize,reduction,angle,speed);
        _toVector = CustomAngle.VectorRotation(angle);
        _speed = speed;

    }

    
}
