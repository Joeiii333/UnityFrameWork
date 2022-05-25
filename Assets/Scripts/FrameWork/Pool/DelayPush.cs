using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayPush : MonoBehaviour
{
    
    //当对象激活时调用
    void OnEnable()
    {
        Invoke("Push",2);
    }

    void Push()
    {
        PoolManager.Instance.PushObj(name,gameObject);
    }
}
