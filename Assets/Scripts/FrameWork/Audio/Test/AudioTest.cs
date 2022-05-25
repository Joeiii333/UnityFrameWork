using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    void OnGUI()
    {
        if(GUI.Button(new Rect(0,0,200,200),"播放音乐" ))
            AudioMgr.Instance.PlayAudio("BackMusic",true);
        
        if(GUI.Button(new Rect(0,200,200,200),"暂停音乐" ))
            AudioMgr.Instance.StopBkMusic();
    }
}
