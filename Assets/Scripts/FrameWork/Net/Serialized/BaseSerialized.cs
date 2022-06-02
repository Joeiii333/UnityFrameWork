using System;
using System.Text;

public abstract class BaseSerialized
{
    /// <summary>
    /// 获取序列化字节数组
    /// </summary>
    public abstract byte[] Writing();

    /// <summary>
    /// 获取序列化后的长度
    /// </summary>
    /// <returns></returns>
    public abstract int GetLength();

    /// <summary>
    /// 子类必须重写，用于返回一个专属的ID以反序列化时区别类信息
    /// </summary>
    /// <returns></returns>
    public abstract int GetID();

    /// <summary>
    /// 反序列化规则
    /// </summary>
    /// <param name="bytes">对应反序列化的字节数组</param>
    /// <param name="beginIndex">开始索引，从该字节数组的第几个位置开始解析 默认是0</param>
    /// <returns>当类嵌套时，通过该返回值确定当前读取类的读取字节长度</returns>
    public abstract int Reading(byte[] bytes, int beginIndex = 0);

    /// <summary>
    /// 添加基础类型的头信息，这里的头信息只用来方便区分基础类型，对于引用类型使用泛型进行区分。
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="index"></param>
    /// <param name="type"></param>
    public void AddHeader(byte[] bytes, ref int index, EnumSimpleValue type)
    {
        //添加头信息
        BitConverter.GetBytes((int) type).CopyTo(bytes, index);
        index += 4;
    }
    
    /// <summary>
    /// 解析头信息
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public EnumSimpleValue ParseHeader(byte[] bytes, ref int index)
    {
        int header = BitConverter.ToInt32(bytes, index);
        index += 4;
        return (EnumSimpleValue) header;
    }

    /// <summary>
    /// 序列化基础类型以及字符串
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="value"></param>
    /// <param name="index"></param>
    public void Serialized(byte[] bytes, object value, ref int index)
    {
        if (value is byte)
        {
            //添加头信息
            AddHeader(bytes, ref index, EnumSimpleValue.Byte);
            bytes[index] = (byte) value;
            index += 1;
            return;
        }

        if (value is string str)
        {
            //添加头信息
            AddHeader(bytes, ref index, EnumSimpleValue.String);

            byte[] strBytes = Encoding.UTF8.GetBytes(str);
            byte[] lenBytes = BitConverter.GetBytes(strBytes.Length);
            lenBytes.CopyTo(bytes, index);
            index += 4;
            strBytes.CopyTo(bytes, index);
            index += strBytes.Length;
            return;
        }


        byte[] serializedBytes = null;
        int len = 0;

        if (value is short s)
        {
            //添加头信息
            AddHeader(bytes, ref index, EnumSimpleValue.Short);
            serializedBytes = BitConverter.GetBytes(s);
            len = sizeof(short);
        }
        else if (value is int i)
        {
            //添加头信息
            AddHeader(bytes, ref index, EnumSimpleValue.Int);
            serializedBytes = BitConverter.GetBytes(i);
            len = sizeof(int);
        }
        else if (value is bool b)
        {
            //添加头信息
            AddHeader(bytes, ref index, EnumSimpleValue.Bool);

            serializedBytes = BitConverter.GetBytes(b);
            len = sizeof(bool);
        }

        serializedBytes.CopyTo(bytes, index);
        index += len;
    }

    /// <summary>
    /// 反序列化基本类型
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public object DeSerialized(byte[] bytes, ref int index)
    {
        EnumSimpleValue simpleValue = ParseHeader(bytes, ref index);

        object info;
        switch (simpleValue)
        {
            case EnumSimpleValue.Byte:
                info = bytes[index];
                index += 1;
                return info;

            case EnumSimpleValue.Bool:
                info = BitConverter.ToBoolean(bytes, index);
                index += sizeof(bool);
                return info;

            case EnumSimpleValue.Short:
                info = BitConverter.ToInt16(bytes, index);
                index += sizeof(short);
                return info;

            case EnumSimpleValue.Int:
                info = BitConverter.ToInt32(bytes, index);
                index += sizeof(int);
                return info;

            case EnumSimpleValue.String:
                int length = bytes[index];
                index += 4;
                info = Encoding.UTF8.GetString(bytes, index, length);
                index += length;
                return info;
        }

        return null;
    }

    /// <summary>
    /// 序列化引用类型
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="data"></param>
    /// <param name="index"></param>
    /// <typeparam name="T">传入的类型</typeparam>
    public void Serialized<T>(byte[] bytes, T data, ref int index) where T : BaseSerialized
    {
        byte[] dataBytes = data.Writing();
        dataBytes.CopyTo(bytes, index);
        index += dataBytes.Length;
    }


    /// <summary>
    /// 反序列化引用类型
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="index"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T DeSerialized<T>(byte[] bytes, ref int index) where T : BaseSerialized, new()
    {
        T obj = new T();
        index += obj.Reading(bytes, index);
        return obj;
    }
}


public enum EnumSimpleValue
{
    Bool = 1,
    Byte = 2,
    Short = 3,
    Int = 4,
    String = 5
}