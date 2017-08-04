using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IRecycleObject
{
    string RecycleID { get; }
    GameObject gameObject { get; }
}

/**
 * 所有 tiled 中配置的对象，都会放入MapPool，并从MapPool 中获取
 */
public class ObjectPool
{
    private static Transform container;
    public static Transform Container
    {
        get {
            if (container == null)
                container = new GameObject("ObjectPool").transform;
            return container;
        }
    }

    private static Dictionary<string, List<GameObject>> cachePool = new Dictionary<string, List<GameObject>>();

    public static void Push(IRecycleObject recycleObj)
    {
        Push(recycleObj.RecycleID, recycleObj.gameObject);
    }

    public static void Push(string objectId, GameObject gameObject)
    {
        List<GameObject> objList;
        cachePool.TryGetValue(objectId, out objList);
        if (objList == null)
        {
            objList = new List<GameObject>();
            cachePool.Add(objectId, objList);
        }
        gameObject.SetActive(false);
        gameObject.transform.SetParent(Container, false);
        objList.Add(gameObject);
    }

    public static GameObject Pull(string objectId)
    {
        if (cachePool.ContainsKey(objectId) && cachePool[objectId].Count > 0)
        {
            GameObject go = cachePool[objectId][0];
            go.SetActive(true);
            cachePool[objectId].RemoveAt(0);
            return go;
        }
        return null;
    }

    public static int GetCount(string objectId)
    {
        if (!cachePool.ContainsKey(objectId))
            return 0;
        return cachePool[objectId].Count;
    }

    public static void Clear()
    {
        foreach(List<GameObject> goList in cachePool.Values)
        {
            for(int i = 0,l = goList.Count;i<l;i++)
            {
                GameObject.Destroy(goList[i]);
            }
        }
        cachePool.Clear();
    }
}