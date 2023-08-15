using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Gun : MonoBehaviour
{
    Vector3 mPosition;
    Vector3 oPosition;
    private Vector3 oriscale;
    [SerializeField] private Transform _transform;
    public float rotateDegree;
    
    private float t = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        oriscale = _transform.localScale;
        oPosition = _transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        mouseTarget();
        //_transform.localEulerAngles += (new Vector3(0, 0, 0) - _transform.localEulerAngles) / 2;
    }

    private float todegree;

    void mouseTarget()
    {
        t += Time.deltaTime;
        mPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rotateDegree = CustomAngle.PointDirection(transform.position, mPosition);
        
       

        var eulerAngles = new Vector3(0f, 0f, todegree);
        _transform.localEulerAngles = eulerAngles;

        var localScale = _transform.localScale;
        if ((eulerAngles.z is > 90f and < 270f) && localScale.y > 0)
            localScale = new Vector3(_transform.localScale.x, -oriscale.y, 0);
        else if((eulerAngles.z is <= 90f or >= 270f) && localScale.y < 0)
            localScale = new Vector3(_transform.localScale.x, oriscale.y, 0);
        
        localScale += (new Vector3(oriscale.x,  (localScale.y>0) ? oriscale.y: -oriscale.y)
                       -localScale)/5f;
        _transform.localScale = localScale;
        
        _transform.localPosition += (oPosition - _transform.localPosition) / 7.5f;

    }

    public void FireEffect(float move = 0.5f, float degree = 25,float size = 0.2f)
    {
        _transform.localScale *= 1+size;
        _transform.localPosition -= CustomAngle.VectorRotation(rotateDegree) * move;
        _transform.localEulerAngles += new Vector3(0,0,(_transform.localEulerAngles.z is  < 90f or > 270f) ? degree:-degree);
    }
    
}
