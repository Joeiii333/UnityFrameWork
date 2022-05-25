using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 公共Mono模块，可以让没有继承Mono的类开启协程，并且实现Update()帧更新
/// </summary>
public class MonoController : MonoBaseManager<MonoController>
{
    private event UnityAction updateEvent;
    void Start()
    {
    }
    void Update()
    {
        if (updateEvent != null)
            updateEvent();
    }
    
    /// <summary>
    /// 给外部添加帧更新函数的方法
    /// </summary>
    /// <param name="fun"></param>
    public void AddUpdateListener(UnityAction fun)
    {
        updateEvent += fun;
    }
    
    /// <summary>
    /// 用于给外部减少帧更新函数的方法
    /// </summary>
    /// <param name="fun"></param>
    public void RemoveUpdateListener(UnityAction fun)
    {
        updateEvent -= fun;
    }
    
}
