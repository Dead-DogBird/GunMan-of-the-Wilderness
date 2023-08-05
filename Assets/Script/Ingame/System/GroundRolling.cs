using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundRolling : MonoBehaviour
{
    [SerializeField] private float Scrollspeed = 20;

    private float width;
    private float oriY;
    private float oriX;
    
    void Start()
    {
        width = GetComponent<BoxCollider2D>().size.x;
        oriX = transform.position.x;
        oriY = transform.position.y;
    }

    void Update()
    {
        transform.Translate(Vector3.left *(Scrollspeed*Time.deltaTime));
        if (transform.position.x <= (width + oriX) * -1)
        {
            Reposition();
        }
    }

    void Reposition()
    {
        Vector2 offset = new Vector2(width * 2,0);
        transform.position = (Vector2)transform.position + offset;
        transform.position = new Vector3(transform.position.x,oriY);

    }
}
