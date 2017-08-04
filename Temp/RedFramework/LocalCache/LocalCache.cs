using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

/// <summary>
/// 
/// </summary>
public class LocalCache
{
    #region Level2 Json封装
    public static string TestJson(object data)
    {
        return JsonUtility.ToJson(data, true);
    }

    public static void SetJson(string key,object data)
    {
        string json = JsonUtility.ToJson(data);
        SetString(key, json);
    }

    public static T GetJson<T>(string key)
    {
        string str = GetString(key);
        T obj = JsonUtility.FromJson<T>(str);
        return obj;
    }
    #endregion

    #region Level1 基础类型封装
    public static void SetBool(string key, bool value)
    {
        SetInt(key, value ? 1 : 0);
    }
    public static bool GetBool(string key)
    {
        return GetInt(key) == 1;
    }

    static public void SetUInt(string key, uint value)
    {
        SetString(key, Convert.ToString(value));
    }

    static public uint GetUInt(string key)
    {
        return Convert.ToUInt32(GetString(key));
    }
    #endregion

    #region Level0 Unity底层封装
    public static bool Contains(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
    public static void Delete(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }
    public static void Save()
    {
        PlayerPrefs.Save();
    }

    public static void SetInt(string key, int value)
    {
        if (Contains(key))
        {
            Delete(key);
        }
        PlayerPrefs.SetInt(key, value);
    }
    public static int GetInt(string key)
    {
        return PlayerPrefs.GetInt(key);
    }

    public static void SetFloat(string key, float value)
    {
        if (Contains(key))
        {
            Delete(key);
        }
        PlayerPrefs.SetFloat(key, value);
    }

    public static float GetFloat(string key)
    {
        return PlayerPrefs.GetFloat(key);
    }

    public static void SetString(string key, string value)
    {
        if (Contains(key))
        {
            Delete(key);
        }
        PlayerPrefs.SetString(key, value);
    }

    static public string GetString(string key)
    {
        return PlayerPrefs.GetString(key);
    }
    #endregion
}
