using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GroundRolling : MonoBehaviour
{
    [SerializeField] private float Scrollspeed = 20;

    private float width;
    private float oriY;
    private float oriX;

    [SerializeField] private Transform firstOffset;
    private float backpostion;
    void Start()
    {
        if (firstOffset == null)
            firstOffset = transform;
        
        
        width = GetComponent<BoxCollider2D>().size.x;
        oriX = transform.localPosition.x;
        oriY = transform.localPosition.y;
        backpostion = (width + firstOffset.localPosition.x) * -1;
    }

    void Update()
    {
        transform.Translate(Vector3.left *(Scrollspeed*Time.deltaTime));
        if (transform.localPosition.x <= backpostion)
        {
            Reposition();
        }
    }

    void Reposition()
    {
        Vector2 offset = new Vector2(width * 2,0);
        transform.localPosition = (Vector2)transform.localPosition + offset;
        transform.localPosition = new Vector3(transform.localPosition.x,oriY);

    }
}
