using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeicalMessage : MonoBehaviour
{
}

//向服务器发送退出消息
public class QuitMessage : BaseSerialized
{
    public override byte[] Writing()
    {
        int index = 0;
        byte[] bytes = new byte[GetLength()];
        return bytes;
    }

    public override int GetLength()
    {
        return 4;
    }

    public override int GetID()
    {
        return 400;
    }

    //不需要反序列化
    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        return 0;
    }
}

//心跳消息，定时发送，用于服务端判断客户端是否处于连接状态
public class HeartMessage : BaseSerialized
{
    public override byte[] Writing()
    {
        int index = 0;
        byte[] bytes = new byte[GetLength()];
        return bytes;
    }

    public override int GetLength()
    {
        return 4;
    }

    public override int GetID()
    {
        return 401;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        throw new System.NotImplementedException();
    }
}