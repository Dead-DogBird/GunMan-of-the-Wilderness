using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundFollow : MonoBehaviour
{
    private Camera _mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(_mainCamera.transform.position.x,transform.position.y);
    }
}
