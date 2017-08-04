using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.IO;
using System.Xml;

/**
 * 负责获取仓库中的资源
 */
public class Repository
{
    private static AssetBundleManifest manifest;//依赖文件
    private static Dictionary<string,string> bundlePathMap;

    private static Dictionary<string, AssetBundle> bundleCache = new Dictionary<string, AssetBundle>();//缓存所有已加载的bundle
    private static Dictionary<string, Object> assetCache = new Dictionary<string, Object>();//缓存所有asset( key= bunleName_assetName )

    //不同平台下StreamingAssets的路径是不同的，这里需要注意一下。
    public static void Load()
    {
        //LoadManifest
        AssetBundle manifestBundle = AssetBundle.LoadFromFile(AssetsPath.StreamingAssets + "StreamingAssets");
        manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        manifestBundle.Unload(false);

        //LoadBundlePathMap
        AssetBundle bundlePathMapBundle = AssetBundle.LoadFromFile(AssetsPath.StreamingAssets + "base/bundlepathmap.ab");
        BundlePathMapAsset bundlePathMapAsset = bundlePathMapBundle.LoadAsset<BundlePathMapAsset>("BundlePathMap");
        bundlePathMap = bundlePathMapAsset.OnAfterDeserialize();
        bundlePathMapBundle.Unload(false);
    }

    #region Super
    /*
    public static T Instantiate<T>(string name)
    {
        return Instantiate(name).GetComponent<T>();
    }
    public static GameObject Instantiate(string name)
    {
        GameObject pref = Repository.GetUIPrefab(name);
        GameObject obj = GameObject.Instantiate(pref);
        return obj;
    }
     */

    public static GameObject GetPrefab(string name)
    {
        return GetMappedResource<GameObject>(name, false);
    }
    public static GameObject GetUIPrefab(string name)
    {
        return GetMappedResource<GameObject>(name, false);
    }
    public static Material GetMaterial(string name)
    {
        return GetMappedResource<Material>(name, false);
    }
    public static Sprite GetSprite(string name)
    {
        return GetMappedResource<Sprite>(name, true);
    }

    //加載本地音樂，不打包
    public static AudioClip GetSoundResource(string path, string aduioname)
    {
        string Url = "file://" + Application.streamingAssetsPath + "/" + path + aduioname;
        WWW www = new WWW(Url);
        if (www.error != null)
            Debug.Log(www.error.ToString() + "...ERROR");
        Debug.Log(Url);
        return www.audioClip;
    }

    public static AudioClip GetSound(string name)
    {
        return GetMappedResource<AudioClip>(name, true);
    }
    #endregion

    ////////base

    /// <summary>
    /// 获取映射过的资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="cacheBundle"></param>
    /// <returns></returns>
    private static T GetMappedResource<T>(string name, bool cacheBundle) where T : Object
    {
        if (!bundlePathMap.ContainsKey(name))
        {
            return null;
        }
        string bundleName = bundlePathMap[name];
        return LoadAndCacheAsset<T>(bundleName, name, cacheBundle);
    }

    /// <summary>
    /// 加载并且缓存Bundle
    /// </summary>
    /// <param name="bundlePath"></param>
    private static AssetBundle LoadAndCacheBundle(string bundlePath, bool cacheBundle)
    {
        AssetBundle bundle;
        bundleCache.TryGetValue(bundlePath, out bundle);
        if (bundle == null)
        {
            //加载依赖的包
            string[] dps = manifest.GetAllDependencies(bundlePath);
            for (int i = 0, l = dps.Length; i < l; i++)
            {
                LoadAndCacheBundle(dps[i], true);
            }
            bundle = AssetBundle.LoadFromFile(AssetsPath.StreamingAssets + bundlePath);

            if (cacheBundle)
                bundleCache.Add(bundlePath, bundle);
        }
        return bundle;
    }
    /// <summary>
    /// 加载并且缓存Asset
    /// </summary>
    /// <param name="bundlePath"></param>
    /// <param name="assetName"></param>
    /// <param name="cacheBundle">是否缓存Bundle，为false的情况是一个Bundle就一个Asset，这样缓存了Asset就不用Bundle</param>
    /// <returns></returns>
    private static T LoadAndCacheAsset<T>(string bundlePath, string assetName, bool cacheBundle) where T : Object
    {
#if UNITY_ANDROID
        //Android 下只认小写
        bundlePath = bundlePath.ToLower();
        assetName = assetName.ToLower();
#endif

        string key = bundlePath + "_" + assetName;
        if (assetCache.ContainsKey(key))
            return assetCache[key] as T;

        AssetBundle bundle = LoadAndCacheBundle(bundlePath, cacheBundle);
        if (bundle == null) return null;

        //Debuger.Log("LoadAndCacheAsset：" + assetName);
        T value = bundle.LoadAsset<T>(assetName);
        assetCache[key] = value;

        //处理Unity的Bug：在编辑器模式下，自定义Shader丢失的问题。参考：http://www.youkexueyuan.com/exp_show/1187.html
#if UNITY_EDITOR
        Type t = typeof(T);
        if (t == typeof(GameObject))
        {
            GameObject go = value as GameObject;
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                string shaderName = r.sharedMaterial.shader.name;
                r.sharedMaterial.shader = Shader.Find(shaderName);
            }
            Projector[] projecters = go.GetComponentsInChildren<Projector>();
            foreach (Projector p in projecters)
            {
                string shaderName = p.material.shader.name;
                p.material.shader = Shader.Find(shaderName);
            }
        }
        else if (t == typeof(Material))
        {
            Material mat = value as Material;
            string shaderName = mat.shader.name;
            mat.shader = Shader.Find(shaderName);
        }
#endif

        //如果Bundle没有缓存，则卸载
        if (!bundleCache.ContainsKey(bundlePath))
            bundle.Unload(false);

        return assetCache[key] as T;
    }
}