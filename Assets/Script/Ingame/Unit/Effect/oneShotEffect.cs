using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oneShotEffect : MonoBehaviour
{
    [SerializeField] private float delay = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,delay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
