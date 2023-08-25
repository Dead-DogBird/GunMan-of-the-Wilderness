using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResponseImage : MonoBehaviour
{
    Vector3 mousePos;
    Vector3 updateMyPos;
    Vector3 zeroPoint;
    Vector3 targetPoint;
    public float focus = 0.735f;
    public float setDrainage = 3.77f;
    public Sprite blured;
    Sprite ori_sprite;
    Image myImage;

    private int now = 0;
    // Start is called before the first frame update
    void Start()
    {
        zeroPoint = transform.localPosition;
        myImage = gameObject.GetComponent<Image>();
        ori_sprite = myImage.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //updateMyPos= gameObject.transform.localPosition;
        //updateMyPos=;
        if (focus >= 0)
            targetPoint = Vector3.Lerp(zeroPoint, mousePos, focus) * setDrainage;
        else
        {
            targetPoint = Vector3.Lerp(zeroPoint, mousePos, -focus) * -setDrainage;
        }
        transform.localPosition += (targetPoint - transform.localPosition) * (Time.unscaledDeltaTime*20);
        if (now != MainMenuManager.Instance.focusCanvas)
        { 
            now = MainMenuManager.Instance.focusCanvas; 
            myImage.sprite = (MainMenuManager.Instance.focusCanvas != 0)? blured : ori_sprite;
        }
        
    }
}
