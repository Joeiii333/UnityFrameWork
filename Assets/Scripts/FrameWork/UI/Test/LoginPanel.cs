using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    void Start()
    {
        Button btn = BaseGetComponent<Button>("Button");
        UIManager.AddCustomEventListener(btn, EventTriggerType.Drag, OnDrag);
    }


    // Update is called once per frame
    void Update()
    {
    }

    private void OnDrag(BaseEventData data)
    {
        data.selectedObject.transform.position = data.currentInputModule.input.mousePosition;
    }

    protected override void OnClick(string btnName)
    {
        switch (btnName)
        {
            case "Button":
                print("123123");
                break;
            case "Button2":
                print("321321");
                break;
        }
    }

    protected override void OnValueChanged(string toggleName, bool value)
    {
        //根据名字判断，处理逻辑        
    }
}