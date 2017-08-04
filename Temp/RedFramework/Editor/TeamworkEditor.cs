using UnityEngine;
using UnityEditor;

public class TeamworkEditor : ScriptableObject
{
    [MenuItem("Red/Teamwork/Reset To Remote")]
    static void ResetToRemote()
    {
        EditorUtility.DisplayDialog("MyTool", "Do It in C# !", "OK", "");
    }
}