using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//월드에 따라서 뒷배경 체인지... 근데 그냥 뭐 스크립트 하나 박아놓고 카메라에 맞춰서 움직이는게 나을덧
public class BackGround : MonoBehaviour
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
    }

    private void LateUpdate()
    {
        transform.position = _mainCamera.transform.position+new Vector3(0,0,20);
        
    }
}
