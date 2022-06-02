using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMessage
{
    public int classID;
    public BaseSerialized messageData;

    private int messageLength { get; set; }

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
        int dataLength = messageData.GetLength() + 4; //因为后续还要加上classID，所以需要+4  !!!!这里要注意，长度是指数据长度，不包含长度标识
        messageLength = dataLength + 4; //这里+4是为了生成字节数组时把长度加上,这里相当于是实际的字节数组长度

        byte[] bytes = new byte[messageLength];
        byte[] data = messageData.Writing();

        BitConverter.GetBytes(dataLength).CopyTo(bytes, index);
        index += 4;
        BitConverter.GetBytes(classID).CopyTo(bytes, index);
        index += 4;
        data.CopyTo(bytes, index);
        MonoBehaviour.print(bytes.Length);
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