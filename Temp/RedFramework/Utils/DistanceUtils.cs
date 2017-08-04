using UnityEngine;
using System.Collections;

public class DistanceUtils
{
    /// <summary>
    /// 获取平面的距离(x,z)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static float PlaneDistance(Vector3 a,Vector3 b)
    {
        return Distance(a.x, a.z, b.x, b.z);
    }

    public static float Distance(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }
    public static float Distance(float ax, float ay, float bx, float by)
    {
        return Mathf.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by));
    }
}
