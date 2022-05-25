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
    private Queue<string> sender = new Queue<string>();
    //接受消息的队列容器，接收线程往里放，主线程取出
    private Queue<string> receiver = new Queue<string>();

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
        if(receiver.Count>0)
            print(receiver.Dequeue()); 
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
            ThreadPool.QueueUserWorkItem(SendMsg);
            //开启接收线程
            ThreadPool.QueueUserWorkItem(ReceiveMsg);
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
    public void Send(string info)
    {
        sender.Enqueue(info);
    }

    private void SendMsg(object obj)
    {
        while (isConnected)
        {
            if (sender.Count > 0)
            {
                socket.Send(Encoding.UTF8.GetBytes(sender.Dequeue()));
            }
        }
    }

    //不停的接受消息
    private void ReceiveMsg(object obj)
    {
        while (isConnected)
        {
            if(socket.Available > 0)
            {
                receiveNum = socket.Receive(receiveBytes);
                //收到消息 解析消息为字符串 并放入公共容器
                receiver.Enqueue(Encoding.UTF8.GetString(receiveBytes, 0, receiveNum));
            }    
        }
    }
    
    public void Close()
    {
        if(socket != null)
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