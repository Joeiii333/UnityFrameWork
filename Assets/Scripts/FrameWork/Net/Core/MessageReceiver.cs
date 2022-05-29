using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class MessageReceiver
{
    private Socket socket; //建立连接的Socket

    private bool isConnected; //是否还处于连接状态

    private byte[] cacheBytes = new byte[1024 * 1024]; //接受字节数组缓冲区
    private int cacheLength = 0; //缓冲区当前长度，用来判断分包黏包
    private Queue<BaseMessage> receiverQueue = new Queue<BaseMessage>(); //存放消息的队列

    private MessageReceiver()
    {
    }

    private static MessageReceiver instance = new MessageReceiver();

    public static MessageReceiver Instance => instance;


    public MessageReceiver StartUp(Socket socket)
    {
        this.socket = socket;
        isConnected = true;
        MonoBehaviour.print(socket.Connected);
        MonoBehaviour.print(socket.RemoteEndPoint);

        //开启接受消息线程
        ThreadPool.QueueUserWorkItem(ReceiveMessage);
        return Instance;
    }

    private void ReceiveMessage(object state)
    {
        while (isConnected)
        {
            if (socket.Available > 0)
            {
                MonoBehaviour.print("收到消息");
                byte[] receiveBytes = new byte[1024 * 1024]; //节省内存
                int receiveLength = socket.Receive(receiveBytes);
                //收到消息，先处理消息再放入容器中
                HandleReceiveMessage(receiveBytes, receiveLength);
            }
        }
    }

    /// <summary>
    /// 在此处验证消息完整性(分包黏包),类型处理
    /// </summary>
    private void HandleReceiveMessage(byte[] receiveBytes, int receiveLength)
    {
        int msgLength = 0;
        int nowIndex = 0;

        //每次收到新的消息，应该先存入缓存中后操作缓存数组
        receiveBytes.CopyTo(cacheBytes, cacheLength);
        cacheLength += receiveLength;

        while (true)
        {
            //每次将长度设置为-1 是避免上一次解析的数据 影响这一次的判断
            msgLength = -1;

            if (cacheLength - nowIndex >= 4)
            {
                //解析长度
                msgLength = BitConverter.ToInt32(cacheBytes, nowIndex);
                nowIndex += 4;
            }

            //可以解析完整的消息体
            if (cacheLength - nowIndex >= msgLength)
            {
                //解析classID
                BaseMessage msg = new BaseMessage();
                msg.classID = BitConverter.ToInt32(cacheBytes, nowIndex);
                nowIndex += 4;

                //解析数据data
                msg.messageData = ParseClassByID(cacheBytes, nowIndex, msg.classID);

                //如果消息体完整且成功解析，加入队列
                if (!msg.IsEmpty())
                {
                    receiverQueue.Enqueue(msg);
                    nowIndex += msg.messageData.GetLength();
                }
                else
                {
                    nowIndex -= 4; //如果消息体解析失败，索引回退
                    break;
                }


                //正好解析完
                if (nowIndex == cacheLength)
                {
                    cacheLength = 0;
                    break;
                }
            }
            else
            {
                //如果解析不出完整的消息，证明存在分包
                if (msgLength != -1)
                    //如果进行了长度的解析，但没有解析出消息体，需要将解析长度的index回退
                    nowIndex -= 4;
                //把剩余没有解析的字节数组内容移动到前面，用于缓存下次继续解析
                Array.Copy(cacheBytes, nowIndex, cacheBytes, 0, cacheLength - nowIndex);
                cacheLength = cacheLength - nowIndex;
                break;
            }
        }
    }

    private BaseSerialized ParseClassByID(byte[] bytes, int nowIndex, int classID)
    {
        switch (classID)
        {
            case 1000:
                Person p = new Person();
                p.Reading(bytes, nowIndex);
                return p;
            default:
                //解析失败
                MonoBehaviour.print("处理数据解析失败，classID:" + classID);
                return null;
        }
    }

    public void ParseMessage()
    {
        //分发消息，classID和data都已经解析出来了
    }

    public int GetReceiverQueueCount()
    {
        return receiverQueue.Count;
    }
}