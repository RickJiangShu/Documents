using UnityEngine;
using System.Collections;

/// <summary>
/// 坐标转换工具集
/// </summary>
public class TransferUtils
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="worldPos"></param>
    /// <param name="canvas"></param>
    /// <returns>localPosition</returns>
    public static Vector2 World2UI(Vector3 worldPos, Camera camera, RectTransform canvas)
    {
        Vector2 viewport = camera.WorldToViewportPoint(worldPos);
        viewport = (viewport - canvas.pivot) * 2;
        float width = canvas.rect.width * 0.5f;
        float height = canvas.rect.height * 0.5f;
        return new Vector2(viewport.x * width, viewport.y * height);
    }

    public static Vector3 UI2World(Vector3 uiPos, Camera camera, RectTransform canvas)
    {
        float width = canvas.rect.width * 0.5f;
        float height = canvas.rect.height * 0.5f;
        Vector3 viewport = new Vector3((uiPos.x / width + 1f) / 2, (uiPos.y / height + 1f) / 2, uiPos.z);
        return camera.ViewportToWorldPoint(viewport);
    }
}
