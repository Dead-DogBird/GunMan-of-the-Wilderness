using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    [SerializeField] private bool triger;

    void Invoke()
    {
        triger = false;
        UIManager.Instance.ShotGunSkill();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (triger)
        {
            Invoke();
        }
    }
}
