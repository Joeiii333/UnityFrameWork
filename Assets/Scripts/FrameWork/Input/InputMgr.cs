using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 输入控制模块，利用事件中心，类似于InputSystem
/// </summary>
public class InputMgr : BaseManager<InputMgr>
{
    private bool isPause = true;

    /// <summary>
    /// 构造函数中，添加Update监听
    /// </summary>
    public InputMgr()
    {
        MonoController.Instance.AddUpdateListener(Update);
    }

    private void Update()
    {
        //如果处于暂停输入检测时，不进行任何检测事件
        if (isPause)
            return;
        CheckKeyCode(KeyCode.W);
        CheckKeyCode(KeyCode.S);
    }

    /// <summary>
    /// 用来检测按键按下抬起分发
    /// </summary>
    /// <param name="key"></param>
    private void CheckKeyCode(KeyCode key)
    {
        //通过事件中心，快速分发事件，类似InputSystem
        if (Input.GetKeyDown(key))
            EventCenter.Instance.EventTrigger("KeyDown", key);
        if (Input.GetKeyUp(key))
            EventCenter.Instance.EventTrigger("KeyUp", key);
    }


    public void IsPause(bool isPause)
    {
        this.isPause = isPause;
    }
}