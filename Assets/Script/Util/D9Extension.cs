using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class D9Extension
{

    public static float Abs(this float num)
    {
        return num > 0 ? num : -num;
    }

    public static float Pow2(this float num)
    {
        return num * num;
    }

    public static int Abs(this int num)
    {
        return num > 0 ? num : -num;
    }

    public static decimal Abs(this decimal num)
    {
        return num > 0 ? num : -num;
    }

    public static float DegreePlus(this float num)
    {
        if (num > 0)
            return num >= 360 ? 360 - num : num;
        else
            return (360 - num);
    }

    public static Vector32 _Vector32(this Vector3 v1, Vector3 v2)
    {
        return new Vector32(v1,v2);
    }
    ///<summary>벡터의 각도(degree)를 알려줍니다</summary>
    public static float ToDegree(this Vector2 pos)
    {
        float angle = (float)Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        if (angle < 0f)
            angle += 360f;
        return angle;
    }

    public static float GetAngle(Vector2 pos)
    {
        float angle = (float)Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        if (angle < 0f)
            angle += 360f;
        return angle;
    }

    // public static float Direction(this Vector3 pos)
    // {
    //     float angle = (float)Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
    //     if (angle < 0f)
    //         angle += 360f;
    //     return angle;
    // }
    //
    // ///<summary>겜메 Length_x,y</summary>
    // public static Vector2 DegreeToVector2(this float angle)
    // {
    //     //반전화
    //     angle = -(angle - 90);
    //
    //     if (angle < 0f)
    //         angle += 360f;
    //
    //     return new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));
    // }

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    public static Vector2 ToVector2(this Vector3 vector3)
    {
        return vector3;
    }

    public static Vector3 ToVector3(this Vector2 vector2)
    {
        return vector2;
    }

    public static T SelectRandom<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    public static float DegreeLerp(float a, float b, float t)
    {
        return Quaternion.Lerp(Quaternion.Euler(0, 0, a), Quaternion.Euler(0, 0, b), t).eulerAngles.z;
    }
}

public class Vector32
{
    public Vector3 vector1;
    public Vector3 vector2;

    public Vector32(Vector3 v1, Vector3 v2)
    {
        vector1 = v1;
        vector2 = v2;
    }
    
}
