using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TeachTcpServerExercises2.Message;

namespace TeachTcpServerExercises2
{
    class ClientSocket
    {
        private static int CLIENT_BEGIN_ID = 1;
        public int clientID;
        public Socket socket;

        private byte[] cacheBytes = new byte[1024 * 1024]; //接受字节数组缓冲区

        private int cacheLength = 0; //缓冲区当前长度，用来判断分包黏包
        // private Queue<BaseMessage> receiverQueue = new Queue<BaseMessage>(); //存放消息的队列

        //上一次收到心跳消息的时间
        private long frontTime = -1;

        //超时时间
        private static int TIME_OUT_TIME = 10;

        public ClientSocket(Socket socket)
        {
            clientID = CLIENT_BEGIN_ID++;
            this.socket = socket;
        }

        private void CheckTimeOut()
        {
            if (frontTime != -1 &&
                DateTime.Now.Ticks / TimeSpan.TicksPerSecond - frontTime >= TIME_OUT_TIME)
                Program.serverSocket.AddDelaySocket(this);
        }

        /// <summary>
        /// 是否是连接状态
        /// </summary>
        public bool Connected => socket.Connected;

        //我们应该封装一些方法
        //关闭
        public void Close()
        {
            if (socket != null)
            {
                Console.WriteLine("clientID:{0}断开连接", clientID);
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket = null;
            }
        }

        //发送
        public void Send(string info)
        {
            if (Connected)
            {
                try
                {
                    socket.Send(Encoding.UTF8.GetBytes(info));
                }
                catch (Exception e)
                {
                    Console.WriteLine("发消息出错" + e.Message);
                    Program.serverSocket.AddDelaySocket(this);
                }
            }
            else
                Program.serverSocket.AddDelaySocket(this);
        }

        //接收
        public void Receive()
        {
            if (!Connected)
            {
                Program.serverSocket.AddDelaySocket(this);
                return;
            }

            try
            {
                if (socket.Available > 0)
                {
                    byte[] result = new byte[1024 * 5];
                    int receiveNum = socket.Receive(result);
                    HandleReceiveMessage(result, receiveNum);
                }

                //检测是否超时
                CheckTimeOut();
            }
            catch (Exception e)
            {
                Console.WriteLine("收消息出错" + e.Message);
                //消息解析错误也要断开
                Program.serverSocket.AddDelaySocket(this);
            }
        }

        private void HandleMessage(object obj)
        {
            BaseMessage msg = obj as BaseMessage;
            if (msg.messageData is Person p)
            {
                //执行逻辑
                Console.WriteLine(p.name);
            }
            else if (msg.messageData is QuitMessage)
            {
                //收到退出消息
                Program.serverSocket.AddDelaySocket(this);
            }
            else if (msg.messageData is HeartMessage)
            {
                //收到心跳消息 , 记录收到消息的时间
                frontTime = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
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
                        // receiverQueue.Enqueue(msg);
                        ThreadPool.QueueUserWorkItem(HandleMessage, msg);
                        nowIndex += msg.messageData.GetLength();
                    }
                    else
                        nowIndex -= 4; //如果消息体解析失败，索引回退

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
            BaseSerialized baseMsg = null;
            switch (classID)
            {
                case 1000:
                    baseMsg = new Person();
                    baseMsg.Reading(bytes, nowIndex);
                    return baseMsg;
                case 2000:
                    baseMsg = new Person();
                    baseMsg.Reading(bytes, nowIndex);
                    return baseMsg;
                case 400:
                    //退出信息
                    baseMsg = new QuitMessage();
                    return baseMsg;
                case 401:
                    //心跳消息
                    baseMsg = new HeartMessage();
                    Console.WriteLine("收到心跳消息");
                    return baseMsg;
                default:
                    //解析失败
                    Console.WriteLine("处理数据解析失败，classID:{0}", classID);
                    return baseMsg;
            }
        }
    }
}