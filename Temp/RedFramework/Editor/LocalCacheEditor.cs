using UnityEngine;
using UnityEditor;

public class LocalCacheEditor : ScriptableObject
{
    [MenuItem("Red/Local Cache/Delete All")]
    static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
}