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
               sizeof(int) * 6; //5个属性的协议头+1个自己本身的协议头
    }

    public override int GetID()
    {
        return 1000;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        age = (int) DeSerialized(bytes, ref index);
        name = (string) DeSerialized(bytes, ref index);
        id = (short) DeSerialized(bytes, ref index);
        sex = (bool) DeSerialized(bytes, ref index);
        info = DeSerialized<PlayerInfo>(bytes, ref index);
        return index - beginIndex;
    }

    public override byte[] GetBytes()
    {
        int index = 0;
        byte[] bytes = new byte[GetLength()];    
        Serialized(bytes, GetID(), ref index);     //加入自己本身的类型的标识ID
        Serialized(bytes, age, ref index);
        Serialized(bytes, name, ref index);
        Serialized(bytes, id, ref index);
        Serialized(bytes, sex, ref index);
        Serialized<PlayerInfo>(bytes, info, ref index);
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
        Serialized(bytes, GetID(), ref index);    //加入自己本身的类型的标识ID
        Serialized(bytes, lv, ref index);
        return bytes;
    }

    public override int GetLength()
    {
        return sizeof(int) +
               sizeof(int) * 2; //字段协议头+GetID的协议头 
    }

    public override int GetID()
    {
        return 1001;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        lv = (int) DeSerialized(bytes, ref index);

        return index - beginIndex;
    }
}