using UnityEngine;
using System.Collections;

/// <summary>
/// 路径配置（相对于Assets目录下的路径）
/// </summary>
public class RelativePath //RepositoryPath
{
    public static string StreamingAssets =
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        "/StreamingAssets/";
#elif UNITY_ANDROID
		"!assets/";
#elif UNITY_IPHONE
		"/Raw/";
#else
        string.Empty;
#endif

    public static string Framework = "RedFramework/";
    public static string Repository = "Repository/";


    //配置
    public static string ConfigRepository = Repository + "Config/";
    public static string ConfigOutput = Repository + "ConfigOutput/";//配置输出目录


    public static string PrefabRepository = Repository + "Prefabs/";
    public static string Materials = Repository + "Materials/";

    public static string UIRepository = Repository + "UI/";
    public static string UIPrefabRepository = UIRepository + "Prefabs/";
    public static string Icons = UIRepository + "Icons/";
    public static string Sound = Repository + "Sound/";

#if TCP
    //协议
    public static string ProtocolLibrary = Repository + "Protocol/";
    public static string ProtocolOutput = CommonScripts + "Protocol/";
#endif

    //通信代理
    //  public static string ProxyOutput = CommonScripts + "Proxy/";//Proxy 是根据协议自动生成的

}

public class AssetsPath
{
    public static string StreamingAssets = Application.dataPath + RelativePath.StreamingAssets;
   // public static string BundlePathMap = Application.dataPath + "/"
}
