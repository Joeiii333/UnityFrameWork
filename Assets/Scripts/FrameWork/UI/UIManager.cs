using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// UI层级
/// </summary>
public enum E_UI_Layer
{
    Bot,
    Mid,
    Top,
    System
}

public class UIManager : BaseManager<UIManager>
{
    public Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

    //记录UI的Canvas对象，方便外部使用
    public RectTransform canvas;

    //定义UI所在Canvas下的四个层级
    private Transform bot;
    private Transform mid;
    private Transform top;
    private Transform system;

    public UIManager()
    {
        //加载Canvas
        GameObject obj = ResMgr.Instance.Load<GameObject>("Canvas");
        canvas = obj.transform as RectTransform;

        bot = canvas.Find("Bot");
        mid = canvas.Find("Mid");
        top = canvas.Find("Top");
        system = canvas.Find("System");
        GameObject.DontDestroyOnLoad(obj);
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <param name="panelName">面板名</param>
    /// <param name="layer">面板所在的UI层级</param>
    /// <param name="callBack">当面板的预设体创建完成后，需要执行的回调</param>
    /// <typeparam name="T">面板脚本类型</typeparam>
    public void ShowPanel<T>(string panelName, E_UI_Layer layer = E_UI_Layer.Mid, UnityAction<T> callBack = null)
        where T : BasePanel
    {
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].ShowMe();
            if (callBack != null)
            {
                callBack(panelDic[panelName] as T);
            }
        }
        else
        {
            //异步加载对应的面板预制体对象
            ResMgr.Instance.LoadAsync<GameObject>(panelName, (obj) =>
            {
                //将他作为Canvas的子对象
                //并且设置相对位置
                Transform fatherTrans = bot;
                switch (layer)
                {
                    case E_UI_Layer.Mid:
                        fatherTrans = mid;
                        break;
                    case E_UI_Layer.Top:
                        fatherTrans = top;
                        break;
                    case E_UI_Layer.System:
                        fatherTrans = system;
                        break;
                }

                //设置父对象和相对大小位置
                obj.transform.SetParent(fatherTrans);

                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;

                (obj.transform as RectTransform).offsetMax = Vector2.zero;
                (obj.transform as RectTransform).offsetMin = Vector2.zero;
                //得到预设体上的面板脚本
                T panel = obj.GetComponent<T>();

                //这里传入一个委托回调函数是因为，异步加载并不会在这一帧得到该脚本对象
                //异步加载这里传入的Lambda表达式就已经代表异步加载完成后执行的函数了
                //所以需要等加载完成后执行获得脚本对应逻辑
                if (callBack != null)
                    callBack(panel);
                panel.ShowMe();
            });
        }
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="panelName"></param>
    public void HidePanel(string panelName)
    {
        if (panelDic.ContainsKey(panelName))
        {
            GameObject.Destroy(panelDic[panelName].gameObject);
            panelDic.Remove(panelName);
            panelDic[panelName].HideMe();
        }
    }

    /// <summary>
    /// 得到一个已经加载完成的面板，方便外部使用
    /// </summary>
    /// <param name="panelName">面板名</param>
    /// <typeparam name="T">面板类型</typeparam>
    /// <returns></returns>
    public T GetPanel<T>(string panelName) where T : BasePanel
    {
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;
        return null;
    }

    /// <summary>
    /// 根据层级枚举返回对应的层级对象
    /// </summary>
    /// <param name="layer">层级枚举</param>
    /// <returns></returns>
    public Transform GetLayerInCanvas(E_UI_Layer layer)
    {
        switch (layer)
        {
            case E_UI_Layer.Bot:
                return bot;
            case E_UI_Layer.Mid:
                return mid;

            case E_UI_Layer.Top:
                return top;

            case E_UI_Layer.System:
                return system;
        }

        return null;
    }

    /// <summary>
    /// 给某一个控件添加一个EventTrigger自定义事件
    /// </summary>
    /// <param name="component">组件</param>
    /// <param name="type">事件类型</param>
    /// <param name="callBack">事件函数</param>
    public static void AddCustomEventListener(UIBehaviour component, EventTriggerType type,
        UnityAction<BaseEventData> callBack)
    {
        EventTrigger trigger = component.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = component.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);

        trigger.triggers.Add(entry);
    }
}