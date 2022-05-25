using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 可以提供给外部添加帧更新事件的方法
/// 可以提供给外部添加协程的方法
/// </summary>
public class MonoManager : BaseManager<MonoManager>
{
    private MonoController controller;

    public MonoManager()
    {
        //保证了MonoController对象的唯一性
        GameObject obj = new GameObject("MonoController");
        controller = obj.AddComponent<MonoController>();
    }
    
    /// <summary>
    /// 封装外部添加帧更新函数的方法
    /// </summary>
    /// <param name="fun"></param>
    public void AddUpdateListener(UnityAction fun)
    {
        controller.AddUpdateListener(fun);
    }
    
    /// <summary>
    /// 封装给外部减少帧更新函数的方法
    /// </summary>
    /// <param name="fun"></param>
    public void RemoveUpdateListener(UnityAction fun)
    {
        controller.RemoveUpdateListener(fun);
    }
    
    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return controller.StartCoroutine(routine);
    }
    
}
