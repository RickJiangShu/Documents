using UnityEngine;
using System.Collections;

/**
 * 摄像机跟随脚本
 * 
 * 使用说明：将主摄像机放置在此GameObject对象下面，因为此脚本将直接对其到英雄坐标。
 */
public class CameraFollow : MonoBehaviour
{
    //跟随
    public Transform target;

    //震动
    private const float amplitude = 0.1f;//振幅
    private const float duration = 0.5f;
    private float shankTime = 0f;
    private bool isShanking = false;

    // Use this for initialization
    void Start()
    {
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position;
        }


        //震屏效果
        if (isShanking)
        {
            transform.position += Random.insideUnitSphere * amplitude;
            shankTime -= Time.deltaTime;
            if (shankTime < 0)
            {
                isShanking = false;
            }
        }
    }

    /// <summary>
    /// 开启震动
    /// </summary>
    public void Shake()
    {
        isShanking = true;
        shankTime = duration;
    }
}
