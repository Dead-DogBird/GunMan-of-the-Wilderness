using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SetCursor : MonoBehaviour
{
    Vector3 mouse; 
    RectTransform R_tm;
    void Start()
    {
        R_tm=GetComponent<RectTransform>();
        Cursor.visible = false;
    }
    void FixedUpdate()
    {
    }
    void Update()
    {
        
        mouse=Input.mousePosition;
        R_tm.position=mouse+new Vector3(0,0,10);
    }
}
