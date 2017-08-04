using UnityEngine;
using System.Collections;

/**
 * 用于将 new Vector3() new string[]{} 这些分配临时内存的操作封装，以免触发gc
 */
public class CacheObjects
{
    public static Vector2 vec2 = Vector2.zero;
    public static Vector2 GetVector2(float x, float y)
    {
        vec2.x = x;
        vec2.y = y;
        return vec2;
    }
    public static Vector3 vec3 = Vector3.zero;
    public static Vector3 GetVector3(float x, float y, float z)
    {
        vec3.x = x;
        vec3.y = y;
        vec3.z = z;
        return vec3;
    }

    public static int[] xy = new int[2];//即Tile
    public static int[] GetTile(int x, int y)
    {
        xy[0] = x;
        xy[1] = y;
        return xy;
    }

    /*
    public static Tile tile = new Tile(0, 0);
    public static Tile GetTile(int x, int y)
    {
        tile.x = x;
        tile.y = y;
        return tile;
    }
     */

    private static string[] strArr = new string[1];
    private static string[] strArr2 = new string[2];
    private static string[] strArr3 = new string[3];
    public static string[] GetStringArray(string s1)
    {
        strArr[0] = s1;
        return strArr;
    }
    public static string[] GetStringArray(string s1,string s2)
    {
        strArr2[0] = s1;
        strArr2[1] = s2;
        return strArr2;
    }
    public static string[] GetStringArray(string s1,string s2,string s3)
    {
        strArr3[0] = s1;
        strArr3[1] = s2;
        strArr3[2] = s3;
        return strArr3;
    }
}
