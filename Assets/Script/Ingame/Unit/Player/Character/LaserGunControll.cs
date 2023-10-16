using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGunControll : MonoBehaviour
{
    private Animator _animator;
    private bool varbool;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void Setbool()
    {
        _animator.SetTrigger("isStartover");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
