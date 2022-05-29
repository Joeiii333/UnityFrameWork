using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMessage
{
    // public int messageLength;
    public int classID;
    public BaseSerialized messageData;
    private int messageLength;

    public BaseMessage()
    {
    }

    public BaseMessage(BaseSerialized data)
    {
        messageData = data;
        classID = data.GetID();
    }

    /// <summary>
    /// 将需要发送的信息打包，添加上信息长度和类型头
    /// 信息长度 = data长度 + 类型头长度
    /// </summary>
    /// <returns></returns>
    public byte[] GetMessagePacket()
    {
        int index = 0;
        int length = messageData.GetLength() + 8;            //因为后续还要加上classID和长度标识，所以需要+8；
        messageLength = length;

        byte[] bytes = new byte[length];
        byte[] data = messageData.GetBytes();
        
        BitConverter.GetBytes(length).CopyTo(bytes, index);
        index += 4;
        BitConverter.GetBytes(classID).CopyTo(bytes, index);
        index += 4;
        data.CopyTo(bytes, index);
        return bytes;
    }

    public bool IsEmpty()
    {
        return classID == 0 || messageData == null;
    }

    public int GetMessageLength()
    {
        return messageLength;
    }
}