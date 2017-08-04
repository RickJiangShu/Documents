using UnityEngine;
using System.Collections;

/**
 * 对象工厂，地图上所有对象的生成皆通过这里，然后销毁调用RecycleObject.Recycle 进行回收。
 */
public class ObjectFactory
{
    //base
    private static GameObject InstanceGameObject(string id)
    {
        GameObject pref = Repository.GetPrefab(id);
        if (pref == null)
        {
            Debug.LogError("找不到Prefab：" +id);
            return null;
        }
        GameObject obj = GameObject.Instantiate(pref);
        return obj;
    }

    //super
    public static T GenerateObject<T>(string id)
    {
        return GenerateObject(id).GetComponent<T>();
    }

    public static GameObject GenerateObject(string id)
    {
        GameObject obj = ObjectPool.Pull(id);
        if (obj == null)
        {
            obj = InstanceGameObject(id);
        }
        return obj;
    }


    //生成对象直接放入对象池
    public static void GenerateObjectToPool(string id)
    {
        GameObject obj = InstanceGameObject(id);
        ObjectPool.Push(id, obj);
    }

}
