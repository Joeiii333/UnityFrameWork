using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;

/// <summary>
/// 场景切换模块
/// </summary>
public class SceneMgr : BaseManager<SceneMgr>
{
    /// <summary>
    /// 切换场景（同步）
    /// </summary>
    /// <param name="sceneName">场景名</param>
    /// <param name="action">切换场景后需要执行的方法</param>
    public void LoadScene(string sceneName, UnityAction action)
    {
        SceneManager.LoadScene(sceneName);
        //加载完成后执行
        action();
    }

    /// <summary>
    /// 切换场景（异步），提供给外部的接口，实际调用内部协程
    /// </summary>
    /// <param name="sceneName">场景名</param>
    /// <param name="action">切换场景后需要执行的方法</param>
    public void LoadSceneAsyn(string sceneName, UnityAction action)
    {
        MonoController.Instance.StartCoroutine(ReallyLoadSceneAsyn(sceneName, action));
    }

    IEnumerator ReallyLoadSceneAsyn(string sceneName, UnityAction action)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            //在此处可以更新进度条
            //事件中心向外分发进度情况
            EventCenter.Instance.EventTrigger("UpdateLoading", operation.progress);
            yield return operation.progress;
        }
        
        //加载完成后，执行方法
        action();
    }
}