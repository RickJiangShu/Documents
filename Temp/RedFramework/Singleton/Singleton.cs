using UnityEngine;
using System.Collections;

/// <summary>
/// 单线程单例（参考：http://blog.csdn.net/wuha555/article/details/40983297）
/// 
/// 使用方法：
/// 1、直接继承
/// 2、return Singleton<myclass>.getInstance();  
/// </summary>
public class Singleton<T> where T : class,new()
{
    private static T _instance;
    public static T GetInstance()
    {
        if (_instance == null)
        {
            _instance = new T();
        }
        return _instance;
    }
}

/// <summary>
/// 仅通过接口访问的单例
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="I"></typeparam>
public class Singleton<T,I> where T : class,I,new()
{
    private static T _instance;
    public static I GetInstance()
    {
        if (_instance == null)
        {
            _instance = new T();
        }
        return _instance;
    }
}