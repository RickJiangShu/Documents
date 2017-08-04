using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/**
 * 用于挂载在需要派发动画事件的对象上
 */
public class AnimationEventDispatcher : MonoBehaviour
{
    public event EventHandler<AnimationEventArgs> OnEventDispatch;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Dispatch(string evt)
    {
        OnEventDispatch(this, new AnimationEventArgs(evt));
    }
}
