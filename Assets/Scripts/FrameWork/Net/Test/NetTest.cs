using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetTest : MonoBehaviour
{
    public Button btn1;

    public Button btn2;

    public Button btn3;

    public Button btn4;

    // Start is called before the first frame update
    void Start()
    {
        ConnetServer();
        
        Person p = new Person();
        p.age = 11;
        p.name = "Joe";
        p.id = 1;
        p.sex = false;
        p.info = new PlayerInfo();
        p.info.lv = 99;
        
        Person p2 = new Person();
        p2.age = 22;
        p2.name = "Mark";
        p2.id = 3;
        p2.sex = false;
        p2.info = new PlayerInfo();
        p2.info.lv = 11199;

        //普通发送消息
        btn1.onClick.AddListener(() =>
        {
            BaseMessage message = new BaseMessage(p);
            byte[] bytes = message.GetMessagePacket();
            NetMgr.Instance.Send(message);
            print("发送数据");
        });
        
        //分包
        btn2.onClick.AddListener(() =>
        {
            BaseMessage message = new BaseMessage(p);
            byte[] bytes = message.GetMessagePacket();
            NetMgr.Instance.SendTest(bytes);
        });
        
        //黏包
        btn3.onClick.AddListener(() =>
        {
            BaseMessage message = new BaseMessage(p);
            byte[] bytes = message.GetMessagePacket();
            
            BaseMessage message2 = new BaseMessage(p2);
            message2.classID = 2000;
            byte[] bytes2 = message.GetMessagePacket();
            
            byte[] bytes3 = new byte[message.GetMessageLength() + message2.GetMessageLength()];
            
            bytes.CopyTo(bytes3,0);
            bytes2.CopyTo(bytes3,message.GetMessageLength());
            
            NetMgr.Instance.SendTest(bytes3);

        });
        
        //分包+黏包
        btn4.onClick.AddListener(() =>
        {
            BaseMessage message = new BaseMessage(p);
            byte[] bytes = message.GetMessagePacket();
            NetMgr.Instance.SendTest(bytes);
        });
    }

    // Update is called once per frame
    void Update()
    {
    }

    void ConnetServer()
    {
        NetMgr.Instance.Connet("127.0.0.1",8080);
    }
}