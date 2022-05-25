using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    private void Start()
    {
        Dead();
    }

    void Dead()
    {
        //触发事件
        EventCenter.Instance.EventTrigger("MonsterDead", this);
    }
}