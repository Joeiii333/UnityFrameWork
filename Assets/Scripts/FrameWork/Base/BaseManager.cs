using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 单例模式管理器，想要实现单例模式的非Mono类只需要继承此类即可
/// </summary>
/// <typeparam name="T">单例类的泛型</typeparam>
public class BaseManager<T> where T : new()
{
    private static T instance;

    protected BaseManager() {}

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }

            return instance;
        }
    }
}