using UnityEngine;

/// <summary>
/// 用于希望继承了Mono的单例类
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoBaseManager<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    //继承了Mono脚本的对象不能直接new，Unity内部会实例化
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).Name;
                instance = obj.AddComponent<T>();
                //单例模式对象过场景不移除
                DontDestroyOnLoad(obj);
            }

            return instance;
        }
    }
    
}
