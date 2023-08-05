using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAngle
{
    public static float PointDirection(Vector2 pos1, Vector2 pos2)
    {
        Vector2 pos = pos2 - pos1;
        float angle = (float)Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        if (angle < 0f)
            angle += 360f;
        return angle;
    }

    public static Vector3 VectorRotation(float _angle) {
        
        _angle-=90;
        _angle*=-1;
        if (_angle<0f)
            _angle+=360f;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), Mathf.Cos(_angle * Mathf.Deg2Rad),0);
    }
}
