using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ColliderCallbackController : MonoBehaviour
{
    public event Action<Collider2D> onColiderEnter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        onColiderEnter?.Invoke(other);
    }
}
