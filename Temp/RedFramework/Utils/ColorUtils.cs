using System.Collections;
using UnityEngine;

public class ColorUtils
{
    public static void SetObjectColor(GameObject go, string propertyName, Color color)
    {
        if (go == null) return;

        if (go.GetComponent<Renderer>() != null)
        {
            for (int i = 0; i < go.GetComponent<Renderer>().materials.Length; ++i)
            {
                go.GetComponent<Renderer>().materials[i].SetColor(propertyName, color);
            }
        }
        else
        {
            for (int j = 0; j < go.transform.childCount; ++j)
            {
                SetObjectColor(go.transform.GetChild(j).gameObject, propertyName, color);
            }
        }
    }
}
