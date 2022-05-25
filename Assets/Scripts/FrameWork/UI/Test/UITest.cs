using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.ShowPanel<LoginPanel>("LoginPanel",E_UI_Layer.Mid);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
