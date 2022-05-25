using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI面板基类
/// 找到所有面板下的控件对象
/// 提供 显隐的行为接口
///
/// 快速找到所有的子控件
/// 方便我们在子类中处理逻辑
/// </summary>
public class BasePanel : MonoBehaviour
{
    //UIBehavior是所有UI控件的基类，使用里氏转换原则存储
    //key：对象名字
    //value:该类型的组件
    private Dictionary<string, List<UIBehaviour>> uiDic = new Dictionary<string, List<UIBehaviour>>();

    protected virtual void Awake()
    {
        FindComponentsInChild<Button>();
        FindComponentsInChild<Image>();
        FindComponentsInChild<Text>();
        FindComponentsInChild<Toggle>();
        FindComponentsInChild<ScrollRect>();
        FindComponentsInChild<Slider>();
        FindComponentsInChild<InputField>();
    }

    void Update()
    {
    }
    

    protected virtual void OnClick(string btnName)
    {
    }

    protected virtual void OnValueChanged(string toggleName, bool value)
    {
    }

    /// <summary>
    /// 找到子对象所对应的控件
    /// 这里的思路是
    /// 以Button打比方
    /// 找到父面板节点下的所有Button
    /// 然后将Button所在的GameObject.name为Key生成一个List<UIBehaviour>用于存储所有的UI组件
    /// 当找Image时，相同的GameObject对应的组件也会存入List中
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    void FindComponentsInChild<T>() where T : UIBehaviour
    {
        T[] components = GetComponentsInChildren<T>();
        foreach (var component in components)
        {
            if (uiDic.ContainsKey(component.name))
                uiDic[component.name].Add(component);
            else
                uiDic.Add(component.name, new List<UIBehaviour> {component});

            //如果是按钮控件,添加一个监听事件
            if (component is Button)
                (component as Button).onClick.AddListener(() =>
                {
                    //每次按钮点击调用虚函数OnClick，将按钮名传入虚函数，提供给外部使用
                    //这样处理，子类甚至无需得到Button对象就可以直接为Button添加对应的点击事件
                    OnClick(component.name);
                });

            if (component is Toggle)
                (component as Toggle).onValueChanged.AddListener((value) =>
                {
                    //Toggle带参数也可同样处理
                    OnValueChanged(component.name, value);
                });
        }
    }

    /// <summary>
    /// 子类获取对应的控件
    /// </summary>
    /// <param name="gameObjectName">需要获取控件的物体对象名</param>
    /// <typeparam name="T">组件类型</typeparam>
    /// <returns>该控件</returns>
    protected T BaseGetComponent<T>(string gameObjectName) where T : UIBehaviour
    {
        //如果没有该类型的控件直接返回null
        if (uiDic.ContainsKey(gameObjectName))
        {
            foreach (var c in uiDic[gameObjectName])
            {
                if (c is T)
                {
                    return (c as T);
                }
            }
        }

        return null;
    }

    public virtual void ShowMe()
    {
        gameObject.SetActive(true);
    }

    public virtual void HideMe()
    {
        gameObject.SetActive(false);
    }
}