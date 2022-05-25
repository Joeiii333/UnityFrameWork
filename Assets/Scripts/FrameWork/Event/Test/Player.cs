using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void Awake()
    {
        EventCenter.Instance.AddEventListener("MonsterDead", MonsterDead);
    }

    void MonsterDead()
    {
        print("Monster");
    }

    void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener("MonsterDead", MonsterDead);
    }
}