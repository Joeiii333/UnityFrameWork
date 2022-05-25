using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源加载模块
/// </summary>
public class ResMgr : BaseManager<ResMgr>
{
    /// <summary>
    /// 同步加载资源
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Load<T>(string name) where T : Object
    {
        T res = Resources.Load<T>(name);
        //如果对象是一个GameObject类型，再实例化后直接返回供外部使用
        if (res is GameObject)
        {
            return GameObject.Instantiate(res);
        }

        return res;
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public void LoadAsync<T>(string name, UnityAction<T> callBack) where T : Object
    {
        MonoController.Instance.StartCoroutine(IELoadAsync(name, callBack));
    }
    
    /// <summary>
    /// 协程函数 用于开启异步加载对应的资源
    /// </summary>
    /// <param name="name">资源名</param>
    /// <param name="callBack">回调函数</param>
    /// <typeparam name="T">资源类型</typeparam>
    /// <returns></returns>
    private IEnumerator IELoadAsync<T>(string name, UnityAction<T> callBack) where T : Object
    {
        ResourceRequest loadAsync = Resources.LoadAsync<T>(name);
        yield return loadAsync;
        if (loadAsync.asset is GameObject)
            callBack(GameObject.Instantiate(loadAsync.asset) as T);
        else
            callBack(loadAsync.asset as T);
    }
}