using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Bundle路径映射表
/// </summary>
[System.Serializable]
public class BundlePathMapAsset : ScriptableObject
{
    public List<string> keys = new List<string>();
    public List<string> values = new List<string>();

    public void OnBeforeSerialize(Dictionary<string,string> write)
    {
        keys.Clear();
        values.Clear();

        foreach (var kvp in write)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public Dictionary<string, string> OnAfterDeserialize()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();

        for (int i = 0,l = Math.Min(keys.Count,values.Count); i<l; i++)
            dict.Add(keys[i], values[i]);

        keys.Clear();
        values.Clear();
        keys = null;
        values = null;

        return dict;
    }
}
