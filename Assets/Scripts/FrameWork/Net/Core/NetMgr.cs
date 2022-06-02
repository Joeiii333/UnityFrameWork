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

    //发送心跳消息的间隔时间
    private int SEND_HEART_MSG_TIME = 3;
    private BaseMessage heartMsg = new BaseMessage(new HeartMessage());

    void Awake()
    {
        Init();
        //定时发送心跳消息
        InvokeRepeating("SendHeartMsg", 0, SEND_HEART_MSG_TIME);
    }

    // Update is called once per frame
    void Update()
    {
        if (isConnected && receiver.GetReceiverQueueCount() > 0)
            receiver.HandleMessage();
    } 

    private void Init()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void SendHeartMsg()
    {
        if (isConnected)
            Send(heartMsg);
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
            print("连接成功");
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
    public void Send(BaseMessage msg)
    {
        sender.SendMessage(msg);
    }

    //用于测试，直接发送字节数组的方法
    public void SendTest(byte[] bytes)
    {
        sender.SendMessageTest(bytes);
    }

    public void Close()
    {
        if (socket != null)
        {
            // print("客户端主动断开连接");
            // BaseMessage message = new BaseMessage(new QuitMessage());
            // socket.Send(message.GetMessagePacket());

            socket.Shutdown(SocketShutdown.Both);
            // socket.Disconnect(false);    //不太准
            socket.Close();
            socket = null;

            isConnected = false;
        }
    }

    private void OnDestroy()
    {
        if (isConnected)
            Close();
    }

    public void ParseMessage(BaseMessage msg)
    {
        //msg中的数据已经解析完毕
        //分发消息啊balabala..
    }
}