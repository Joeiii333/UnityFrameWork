using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Other : MonoBehaviour
{
    private void Awake()
    {
        EventCenter.Instance.AddEventListener<Monster>("MonsterDead", OhterWaitMonsterDead);
    }

    void OhterWaitMonsterDead(Monster info)
    {
        print(this);
    }

    void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<Monster>("MonsterDead", OhterWaitMonsterDead);
    }
}