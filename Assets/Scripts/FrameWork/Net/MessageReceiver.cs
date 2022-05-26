using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

public class MessageReceiver
{
    private Socket socket;
    private bool isConnected;
    private int receiveLength;
    private byte[] receiveBytes = new byte[1024 * 1024];

    private MessageReceiver()
    {
    }

    private static MessageReceiver instance = new MessageReceiver();

    public static MessageReceiver Instance => instance;

    private Queue<byte[]> receiverQueue = new Queue<byte[]>();

    public MessageReceiver StartUp(Socket socket)
    {
        this.socket = socket;
        isConnected = true;
        //开启发送线程
        ThreadPool.QueueUserWorkItem(ReceiveMessage);
        return Instance;
    }

    private void ReceiveMessage(object state)
    {
        while (isConnected)
        {
            if (socket.Available > 0)
            {
                receiveLength = socket.Receive(receiveBytes);
                //收到消息，先处理消息再放入容器中
               
            }
        }
    }
}