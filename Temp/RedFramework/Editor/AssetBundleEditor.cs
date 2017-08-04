using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class AssetBundleEditor : ScriptableObject
{
    static string Suffix = ".ab";

    [MenuItem("Red/Asset Bundle/Remove StreamingAssets")]
    static void RemoveStreamingAssets()
    {
        FileUtils.DeleteFolder(RelativePath.StreamingAssets);
    }

    [MenuItem("Red/Asset Bundle/Clear Bundle Name")]
    static void ClearBundleName()
    {
        EditorUtility.DisplayProgressBar("清除Asset Bundle名称", "正在清除Asset Bundle名称中...", 0f);
        string[] bundleNames = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0, length = bundleNames.Length; i < length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(bundleNames[i], true);
            EditorUtility.DisplayProgressBar("清除Asset Bundle名称", "正在清除Asset Bundle名称中...", (float)i / length);
        }

        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    #region Set Bundle Name
    [MenuItem("Red/Asset Bundle/Set Bundle Name")]
    static void SetBundleName()
    {
        SetConfig();

        SetPrefabs();
        SetUIPrefabs();
        SetSound();
        SetMaterials();
        SetIcons();

        EditorUtility.ClearProgressBar();
    }

    static void SetConfig()
    {
        string path = RelativePath.ConfigRepository;
        if (!FileUtils.IsFolderExists(path)) return;

        FileInfo[] txtFiles = FileUtils.GetFiles(path, "*.txt");

        List<FileInfo> all = new List<FileInfo>();
        all.AddRange(txtFiles);
        for (int i = 0, l = all.Count; i < l; i++)
        {
            EditorUtility.DisplayProgressBar("设置Asset Bundle名称", "正在设置Config...", (float)i / l);
            AssetImporter txtImporter = FileUtils.GetImporter(all[i]);
            txtImporter.assetBundleName = "base/config" + Suffix;
        }
    }
    static void SetPrefabs()
    {
        string path = RelativePath.PrefabRepository;
        if (!FileUtils.IsFolderExists(path)) return;

        FileInfo[] prefabs = FileUtils.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
        FileInfo[] fbxs = FileUtils.GetFiles(path, "*.FBX", SearchOption.AllDirectories);

        List<FileInfo> all = new List<FileInfo>();
        all.AddRange(prefabs);
        all.AddRange(fbxs);

        for (int i = 0, l = all.Count; i < l; i++)
        {
            EditorUtility.DisplayProgressBar("设置Asset Bundle名称", "正在设置Prefabs...", (float)i / l);
            string fileName = all[i].Name.Replace(all[i].Extension, "").ToLower();
            string directoryName = all[i].Directory.Name.ToLower();
            AssetImporter importer = FileUtils.GetImporter(all[i]);
            importer.assetBundleName = "prefabs/" + directoryName + "/" + fileName + Suffix;
        }
    }

    static void SetUIPrefabs()
    {
        string path = RelativePath.UIPrefabRepository;
        if (!FileUtils.IsFolderExists(path)) return;

        DirectoryInfo[] dirs = FileUtils.GetDirectories(path);
        for (int i = 0, l = dirs.Length; i < l; i++)
        {
            string dirName = dirs[i].Name;

            string dirPath = path + "/" + dirName + "/";
            string bundleName = "ui/prefabs/" + dirName.ToLower() + Suffix;

            FileInfo[] files = FileUtils.GetFiles(dirPath, "*.prefab", SearchOption.AllDirectories);

            for (int j = 0, k = files.Length; j < k; j++)
            {
                EditorUtility.DisplayProgressBar("设置Asset Bundle名称", "正在设置UIPrefabs...", (float)j / k);
                AssetImporter importer = FileUtils.GetImporter(files[j]);
                importer.assetBundleName = bundleName;
            }
        }
    }

    static void SetSound()
    {
        {
            string path = RelativePath.Sound;
            if (!FileUtils.IsFolderExists(path)) return;

            FileInfo[] mp3Files = FileUtils.GetFiles(path, "*.mp3");

            List<FileInfo> all = new List<FileInfo>();
            all.AddRange(mp3Files);
            for (int i = 0, l = all.Count; i < l; i++)
            {
                EditorUtility.DisplayProgressBar("设置Asset Bundle名称", "正在设置Sound...", (float)i / l);
                AssetImporter mp3Importer = FileUtils.GetImporter(all[i]);
                mp3Importer.assetBundleName = "base/sound" + Suffix;
            }
        }
    }

    static void SetMaterials()
    {
        string path = RelativePath.Materials;
        if (!FileUtils.IsFolderExists(path)) return;

        FileInfo[] files = FileUtils.GetFiles(path, "*.mat");
        for (int i = 0, l = files.Length; i < l; i++)
        {
            string noSuffixName = FileUtils.GetNoSuffixName(files[i]);
            string bundleName = "materials/" + noSuffixName + Suffix;

            AssetImporter importer = FileUtils.GetImporter(files[i]);
            importer.assetBundleName = bundleName;
        }
        
    }
    static void SetIcons()
    {
        string path = RelativePath.Icons;
        if (!FileUtils.IsFolderExists(path)) return;

        DirectoryInfo[] dirs = FileUtils.GetDirectories(path);
        for (int i = 0, l = dirs.Length; i < l; i++)
        {
            string dirName = dirs[i].Name;

            string dirPath = path + "/" + dirName + "/";
            string bundleName = "ui/icons/" + dirName.ToLower() + Suffix;

            FileInfo[] files = FileUtils.GetFiles(dirPath, "*.png", SearchOption.AllDirectories);

            for (int j = 0, k = files.Length; j < k; j++)
            {
                EditorUtility.DisplayProgressBar("设置Asset Bundle名称", "正在设置Icons...", (float)j / k);
                AssetImporter importer = FileUtils.GetImporter(files[j]);
                importer.assetBundleName = bundleName;
            }
        }
    }
    #endregion

    /// <summary>
    /// 用于创建一张Prefabs Name -> BundleName_AssetName的隐射表
    /// </summary>
    [MenuItem("Red/Asset Bundle/Mapping Bundle Path")]
    static void MappingBundlePath()
    {
        //标题
        string tips = "建立Bundle路径映射表";
        EditorUtility.DisplayProgressBar(tips, "正在" + tips + "...", 0f);

        //建表
        BundlePathMapAsset pathMap = ScriptableObject.CreateInstance<BundlePathMapAsset>();
        Dictionary<string, string> dict = new Dictionary<string, string>();

        //prefabs
        FileInfo[] prefabs = FileUtils.GetFiles(RelativePath.PrefabRepository, "*.prefab", SearchOption.AllDirectories);
        FileInfo[] fbxs = FileUtils.GetFiles(RelativePath.PrefabRepository, "*.FBX", SearchOption.AllDirectories);

        //uiprefabs
        FileInfo[] uiPrefabs = FileUtils.GetFiles(RelativePath.UIPrefabRepository, "*.prefab", SearchOption.AllDirectories);
        FileInfo[] materials = FileUtils.GetFiles(RelativePath.Materials, "*.mat", SearchOption.AllDirectories);
        FileInfo[] icons = FileUtils.GetFiles(RelativePath.Icons, "*.png", SearchOption.AllDirectories);
        FileInfo[] sound = FileUtils.GetFiles(RelativePath.Sound, "*.mp3", SearchOption.AllDirectories);

        List<FileInfo> all = new List<FileInfo>();
        all.AddRange(prefabs);
        all.AddRange(fbxs);
        all.AddRange(uiPrefabs);
        all.AddRange(materials);
        all.AddRange(icons);
        all.AddRange(sound);

        for (int i = 0, l = all.Count; i < l; i++)
        {
            EditorUtility.DisplayProgressBar(tips, "正在" + tips + "...", (float)i / l);
            FileInfo prefab = all[i];
            AssetImporter importer = FileUtils.GetImporter(prefab);

            string bundleName = importer.assetBundleName;
            string name = prefab.Name.Replace(prefab.Extension, "");

            if (dict.ContainsKey(name))
            {
                Debug.LogError("相同的Prefab名：" + name);
                continue;
            }
            dict.Add(name, bundleName);
        }
        pathMap.OnBeforeSerialize(dict);

        string o = "Assets/Repository/BundlePathMap.asset";
        AssetDatabase.CreateAsset(pathMap,o);

        AssetImporter mapImporter = AssetImporter.GetAtPath(o);
        mapImporter.assetBundleName = "base/bundlepathmap.ab";

        EditorUtility.ClearProgressBar();

        if (dict.Count == all.Count)
        {
            Debug.Log(tips + "完成！共映射 " + dict.Count + " 个对象。");
        }
        else
        {
            Debug.Log(tips + "失败！当前映射 " + dict.Count + " 个对象。总共需要映射：" + all.Count);
        }
    }


    [MenuItem("Red/Asset Bundle/Build")]
    static void Build()
    {
        BuildTarget targetPlatform = BuildTarget.StandaloneWindows;
#if UNITY_ANDROID
        targetPlatform = BuildTarget.Android;
#elif UNITY_IPHONE
        targetPlatform = BuildTarget.iOS;
#elif UNITY_STANDALONE_WIN
        targetPlatform = BuildTarget.StandaloneWindows;
#endif


        if (!Directory.Exists(AssetsPath.StreamingAssets))
            Directory.CreateDirectory(AssetsPath.StreamingAssets);
        BuildPipeline.BuildAssetBundles(AssetsPath.StreamingAssets, BuildAssetBundleOptions.None, targetPlatform);
        AssetDatabase.Refresh();

        Debug.Log("Asset Bundle打包完成！");
    }


    

}