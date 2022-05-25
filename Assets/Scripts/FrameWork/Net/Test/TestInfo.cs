using System.Text;
using UnityEngine;

public class TestInfo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Person p = new Person();
        p.age = 11;
        p.name = "Joe";
        p.id = 1;
        p.sex = false;
        p.info = new PlayerInfo();
        p.info.lv = 99;
        byte[] sourceByte = p.GetBytes();

        int index = 0;
        Person p2 = new Person();
        p2.Reading(sourceByte);
        print(p2.age);
        print(p2.name);
        print(p2.id);
        print(p2.sex);
        print(p2.info.lv);
    }
    
}


public class Person : BaseSerialized
{
    public int age;
    public string name;
    public short id;

    public bool sex;
    public PlayerInfo info;

    public override int GetLength()
    {
        return sizeof(int) +
               sizeof(int) +
               Encoding.UTF8.GetBytes(name).Length +
               sizeof(short) +
               sizeof(bool) +
               info.GetLength() +
               sizeof(int) * 5; //4个协议头，后续想办法优化计数方式
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        age = (int) Read(bytes, ref index);
        name = (string) Read(bytes, ref index);
        id = (short) Read(bytes, ref index);
        sex = (bool) Read(bytes, ref index);
        info = Read<PlayerInfo>(bytes, ref index);
        return index - beginIndex;
    }

    public override byte[] GetBytes()
    {
        int index = 0;
        byte[] bytes = new byte[GetLength()];
        Write(bytes, age, ref index);
        Write(bytes, name, ref index);
        Write(bytes, id, ref index);
        Write(bytes, sex, ref index);
        Write<PlayerInfo>(bytes, info, ref index);
        return bytes;
    }
}

public class PlayerInfo : BaseSerialized
{
    public int lv;

    public override byte[] GetBytes()
    {
        int index = 0;
        byte[] bytes = new byte[GetLength()];
        Write(bytes, lv, ref index);
        return bytes;
    }

    public override int GetLength()
    {
        return sizeof(int) +
               sizeof(int) * 1; //1个协议头
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        lv = (int) Read(bytes, ref index);

        return index - beginIndex;
    }
}