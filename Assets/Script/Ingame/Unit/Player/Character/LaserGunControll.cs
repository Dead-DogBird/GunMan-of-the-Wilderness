using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGunControll : MonoBehaviour
{
    public bool isDone;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Setbool()
    {
        isDone = true;
    }

    public void Shake()
    {
        IngameCamera.Instance.Shake(0.15f,0,0,1,4f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
