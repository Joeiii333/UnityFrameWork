using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

/// <summary>
/// 音效管理器，用于处理全局的音效
/// </summary>
public class AudioMgr : BaseManager<AudioMgr>
{
    private AudioSource bkMusic;

    private float bkValue = 0.1f;

    private AudioSource audioPlayer;

    private List<AudioSource> audioList = new List<AudioSource>();

    private float audioValue = 0.1f;

    public AudioMgr()
    {
        MonoController.Instance.AddUpdateListener(Update);
    }

    /// <summary>
    /// 利用公共Mono模块检测音效是否播放完毕
    /// </summary>
    private void Update()
    {
        for (int i = audioList.Count - 1; i >= 0; i--)
        {
            if (!audioList[i].isPlaying)
            {
                GameObject.Destroy(audioList[i]);
                audioList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 播放音乐
    /// </summary>
    /// <param name="name"></param>
    public void PlayBkMusic(string name)
    {
        if (bkMusic == null)
        {
            GameObject obj = new GameObject("BkMusic");
            bkMusic = obj.AddComponent<AudioSource>();
        }
        else
        {
            //如果播放的是当前的音频切片,直接播放即可
            if (bkMusic.clip.name == name)
            {
                bkMusic.Play();
                return;
            }
        }

        //异步加载背景音乐，加载完毕后播放
        ResMgr.Instance.LoadAsync<AudioClip>(name, (audioClip) =>
        {
            bkMusic.loop = true;
            bkMusic.clip = audioClip;
            bkMusic.volume = bkValue;
            bkMusic.Play();
        });
    }

    /// <summary>
    /// 暂停音乐
    /// </summary>
    public void PauseBkMusic()
    {
        if (bkMusic != null)
            bkMusic.Pause();
    }

    /// <summary>
    /// 停止音乐
    /// </summary>
    public void StopBkMusic()
    {
        if (bkMusic != null)
            bkMusic.Stop();
    }

    /// <summary>
    /// 改变音量大小
    /// </summary>
    /// <param name="v"></param>
    public void ChangeBkMusicValue(float v)
    {
        if (bkMusic != null)
            bkMusic.volume = v;
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name"></param>
    public void PlayAudio(string name, bool isLoop, UnityAction<AudioSource> callBack = null)
    {
        if (audioPlayer == null)
        {
            GameObject obj = new GameObject("audioPlayer");
            audioPlayer = obj.AddComponent<AudioSource>();
        }
        else
        {
            //如果播放的是当前的音频切片,直接播放即可
            if (audioPlayer.clip.name == name)
            {
                audioPlayer.Play();
                return;
            }
        }

        //异步加载背景音乐，加载完毕后播放
        ResMgr.Instance.LoadAsync<AudioClip>(name, (audioClip) =>
        {
            audioList.Add(audioPlayer);
            audioPlayer.clip = audioClip;
            audioPlayer.volume = audioValue;
            audioPlayer.Play();
            audioPlayer.loop = isLoop;
            if (callBack != null)
                callBack(audioPlayer);
        });
    }


    /// <summary>
    /// 停止音效
    /// </summary>
    public void StopAudio(AudioSource audio)
    {
        if (audioList.Contains(audio))
        {
            audioList.Remove(audio);
            audioPlayer.Stop();
            GameObject.Destroy(audio);
        }
    }

    /// <summary>
    /// 改变音效大小
    /// </summary>
    /// <param name="v"></param>
    public void ChangeAudioValue(float v)
    {
        audioValue = v;
        for (int i = 0; i < audioList.Count; i++)
        {
            audioList[i].volume = audioValue;
        }
    }
}