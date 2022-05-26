using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class NetMgr : MonoBehaviour
{
    private static NetMgr instance;
    public static NetMgr Instance => instance;

    private Socket socket;

    //发送消息的队列容器，主线程将消息放进容器内，发送线程从里取出
    private MessageSender sender;

    //接受消息的队列容器，接收线程往里放，主线程取出
    private MessageReceiver receiver;

    //用于接受消息
    private byte[] receiveBytes = new byte[1024 * 1024];

    //返回收到的字节数
    private int receiveNum;

    //是否连接
    private bool isConnected = false;

    void Awake()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        // if (receiver.Count > 0)

    }

    private void Init()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Connet(string ip, int port)
    {
        //如果是连接状态 直接返回
        if (isConnected)
            return;

        if (socket == null)
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //连接服务端
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        try
        {
            socket.Connect(ipPoint);
            isConnected = true;
            //开启发送线程
            sender = MessageSender.Instance.StartUp(socket);
            //开启接收线程
            receiver = MessageReceiver.Instance.StartUp(socket);
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == 10061)
                print("服务器拒绝连接");
            else
                print("连接失败" + e.ErrorCode + e.Message);
        }
    }

    //发送消息
    public void Send(byte[] bytes)
    {
        sender.SendMessage(bytes);
    }
    
    public void Close()
    {
        if (socket != null)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();

            isConnected = false;
        }
    }

    private void OnDestroy()
    {
        Close();
    }
    
}