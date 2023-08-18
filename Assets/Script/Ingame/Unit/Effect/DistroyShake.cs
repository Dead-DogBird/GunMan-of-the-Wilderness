using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistroyShake : MonoBehaviour
{
    public bool isTrigerd = false;
    public bool isDistroyShake = false;
    public bool triger;

    public float x = 0f;
    public float y = 0.15f;
    public float dir = 0f;
    public float size = 1;

    public float lenth = 15;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnDestroy()
    {
        if(isDistroyShake)
            IngameCamera.Instance.Shake(x, y, dir, size, lenth);
    }

    // Update is called once per frame
    void Update()
    {
        if (isTrigerd)
        {
            Shake();
        }
    }

    void Shake()
    {
        if (!triger) return;
        triger = false;
        IngameCamera.Instance.Shake(x, y, dir, size, lenth);
    }
}