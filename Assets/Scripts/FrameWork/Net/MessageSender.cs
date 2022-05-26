﻿using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class MessageSender
{
    
    private Socket socket;
    private bool isConnected;
    private MessageSender(){}

    private static MessageSender instance = new MessageSender();

    public static MessageSender Instance => instance;

    private Queue<byte[]> sendQueue = new Queue<byte[]>();
    
    public MessageSender StartUp(Socket socket)
    {
        this.socket = socket;
        isConnected = true;
        //开启发送线程
        ThreadPool.QueueUserWorkItem(SendMessage);
        return Instance;
    }

    private void SendMessage(object state)
    {
        while (isConnected)
        {
            if (sendQueue.Count > 0)
            {
                // socket.Send();
            }
        }
    }

    public void SendMessage(byte[] bytes)
    {
        sendQueue.Enqueue(bytes);
    }
}
