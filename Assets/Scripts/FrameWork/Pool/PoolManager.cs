using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 池子中的一列容器
/// </summary>
public class PoolData
{
    //实例化本身
    public GameObject Obj;

    //对象的容器
    public List<GameObject> poolList;

    public PoolData(GameObject obj, GameObject poolObj)
    {
        Obj = new GameObject(obj.name);
        Obj.transform.parent = poolObj.transform;
        poolList = new List<GameObject>() {obj};
    }

    /// <summary>
    /// 面向对象的思想来实现将对象压入对象池
    /// </summary>
    /// <param name="obj"></param>
    public void PushObj(GameObject obj)
    {
        obj.SetActive(false);
        poolList.Add(obj);
        obj.transform.parent = Obj.transform;
    }

    /// <summary>
    /// 从单列数据中取出对象池数据
    /// </summary>
    /// <returns></returns>
    public GameObject GetObj()
    {
        GameObject obj;
        obj = poolList[0];
        poolList.RemoveAt(0);
        //断开父子关系
        obj.transform.parent = null;
        obj.SetActive(true);
        return obj;
    }
}

/// <summary>
/// 缓存池模块
/// </summary>
public class PoolManager : BaseManager<PoolManager>
{
    //缓存池容器
    public Dictionary<string, PoolData> objPool = new Dictionary<string, PoolData>();

    //设置缓存池对象
    private GameObject pool;

    /// <summary>
    /// 从缓存池中取GameObject
    /// </summary>
    /// <param name="name">需要从对象池中取出的对象路径</param>
    /// <returns></returns>
    public void GetObj(string name, UnityAction<GameObject> callback)
    {
        if (objPool.ContainsKey(name) && objPool[name].poolList.Count > 0)
        {
            callback(objPool[name].GetObj());
        }
        else
        {
            ResMgr.Instance.LoadAsync<GameObject>(name, (obj) =>
            {
                obj.name = name;
                callback(obj);
            });
        }
    }

    /// <summary> 
    /// 将新的对象放入缓存池中
    /// </summary>
    /// <param name="name"></param>
    /// <param name="obj"></param>
    public void PushObj(string name, GameObject obj)
    {
        if (pool == null)
        {
            pool = new GameObject("Pool");
        }

        //不存在对应的数组
        if (!objPool.ContainsKey(name))
        {
            objPool.Add(name, new PoolData(obj, pool));
        }

        objPool[name].PushObj(obj);
    }

    /// <summary>
    /// 清空缓存池，主要用于场景切换
    /// </summary>
    public void Clear()
    {
        objPool.Clear();
        pool = null;
    }
}