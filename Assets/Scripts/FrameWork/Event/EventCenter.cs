using System;
using System.Collections.Generic;
using UnityEngine.Events;

public interface IEventInfo
{
}

public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}

public class EventInfo : IEventInfo
{
    public UnityAction actions;

    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件中心
/// 用于传递项目中的事件，降低耦合性
/// </summary>
public class EventCenter : BaseManager<EventCenter>
{
    //定义可变参数类型的委托，用于传递多种方法
    // public delegate void EventMgr(params object[] param);

    //key:事件名
    //value：对应监听事件的委托
    public readonly Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="name">事件名</param>
    /// <param name="action">用来处理事件的委托函数</param>
    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo<T>(action));
        }
    }

    /// <summary>
    /// 添加事件监听 重载，委托不需要参数
    /// </summary>
    /// <param name="name">事件名</param>
    /// <param name="action">用来处理事件的委托函数</param>
    public void AddEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo(action));
        }
    }

    /// <summary>
    /// 事件触发
    /// </summary>
    /// <param name="name">哪一个事件触发</param>
    public void EventTrigger<T>(string name, T info)
    {
        if (eventDic.ContainsKey(name))
        {
            //依次执行委托
            (eventDic[name] as EventInfo<T>).actions(info);
        }
    }
    
    /// <summary>
    /// 事件触发，无参委托触发
    /// </summary>
    /// <param name="name">哪一个事件触发</param>
    public void EventTrigger(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            //依次执行委托
            (eventDic[name] as EventInfo).actions();
        }
    }


    /// <summary>
    /// 移除对应的事件监听
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions += action;
        }
    }
    
    /// <summary>
    /// 移除对应的事件监听，委托不需要参数
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions += action;
        }
    }

    /// <summary>
    /// 当过场景时，清空事件中心
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
}