using UnityEngine;
using System;
using System.Collections;

public class AnimationEventArgs : EventArgs
{
    private string eventType;
    public string EventType { get { return eventType; } }
    public AnimationEventArgs(string eventType)
    {
        this.eventType = eventType;
    }
}
