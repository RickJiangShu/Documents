using System;
using System.Collections;
using System.Collections.Generic;

public delegate void EventCallBack();

public class EventManager
{
    private static readonly Dictionary<string, Delegate> listeners = new Dictionary<string, Delegate>();

    public static void Add<T>(string evt, Action<T> callback)
    {
        AddListener(evt, callback);
    }

    public static void Add(string evt,Action callback)
    {
        AddListener(evt, callback);
    }


    public static void Remove(string evt,Action callback)
    {
        Delegate d;
        if (listeners.TryGetValue(evt, out d))
        {
            Delegate list = Delegate.Remove(d, callback);

            if (list == null)
            {
                listeners.Remove(evt);
            }
            else
            {
                listeners[evt] = list;
            }
        }
    }

    public static void Dispatch<T>(string evt, T data)
    {
        Delegate d;
        if (listeners.TryGetValue(evt, out d))
        {
            (d as Action<T>)(data);
        }
    }

    public static void Dispatch(string evt)
    {
        Delegate d;
        if (listeners.TryGetValue(evt, out d))
        {
            (d as Action)();
        }
    }

    //base
    private static void AddListener(string evt, Delegate callback)
    {
        Delegate d;
        if (listeners.TryGetValue(evt, out d))
        {
            listeners[evt] = Delegate.Combine(d, callback);
        }
        else
        {
            listeners[evt] = callback;
        }
    }
    private static Delegate GetListener(string evt)
    {
        if (string.IsNullOrEmpty(evt))
        {
            throw new ArgumentNullException();
        }
        Delegate d;
        if (listeners.TryGetValue(evt, out d))
        {
            return d;
        }
        return null;
    }
}
