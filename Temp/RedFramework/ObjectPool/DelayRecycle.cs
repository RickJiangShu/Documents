using UnityEngine;
using System.Collections;

/// <summary>
/// 延迟回收
/// </summary>
public class DelayRecycle : MonoBehaviour
{
    public float m_Delay;
    public string objectId;

    public void OnEnable()
    {
        Invoke("Recycle", m_Delay);
    }

    public void Recycle()
    {
        ObjectPool.Push(objectId, gameObject);
    }

}
