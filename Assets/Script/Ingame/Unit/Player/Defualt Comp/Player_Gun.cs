using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Gun : MonoBehaviour
{
    Vector3 mPosition;
    Vector3 oPosition;
    Vector3 target;
    private Vector3 oriscale;
    [SerializeField] private Transform _transform;
    public float rotateDegree;
    // Start is called before the first frame update
    void Start()
    {
        oriscale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        mouseTarget();
    }
    void mouseTarget()
    {
        
        mPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        oPosition = transform.localPosition;
        target = mPosition - oPosition;
        rotateDegree = -1 * Mathf.Atan2(target.x, target.y) * Mathf.Rad2Deg + 90;
        _transform.rotation = Quaternion.Euler(0f, 0f, rotateDegree);
        
        if (Mathf.Abs(rotateDegree) > 90)
        {
            _transform.localScale = new Vector3(oriscale.x, -oriscale.y);
        }
        else
        {
            _transform.localScale = new Vector3(oriscale.x, oriscale.y);
        }
    }
}
