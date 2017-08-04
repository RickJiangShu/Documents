using UnityEngine;
using System.Collections;
using System;

public class Language
{
    public delegate string GetLocalDelegate(string langID);
    public static GetLocalDelegate GetLocal;//需要由核心逻辑指定委托函数，通常为LanguageConfig.Get


    public static Action onChanged;//当语言发生改变
    private static string current = "en";//语言 cn en
    public static string Current
    {
        get { return current; }
        set
        {
            current = value;
            if (onChanged != null) onChanged();
        }
    }
}
