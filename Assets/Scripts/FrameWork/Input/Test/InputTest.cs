using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour
{
    private void Awake()
    {
        EventCenter.Instance.AddEventListener<KeyCode>("KeyDown", KeyDown);
        EventCenter.Instance.AddEventListener<KeyCode>("KeyUp", KeyUp);

    }

    // Start is called before the first frame update
    void Start()
    {
        //开启输入检测
        InputMgr.Instance.IsPause(false);
    }

    void KeyDown(KeyCode key)
    {
        print(key+"键按下");
    }
    
    void KeyUp(KeyCode key)
    {
        print(key+"键抬起");
    }
}
