using System;
using System.Text;

public class Deserializer
{
    int startIndex;
    byte[] bytes;

    public Deserializer(byte[] bs, int offset = 0)
    {
        startIndex = offset;
        bytes = bs;
    }

    public byte ToByte()
    {
        var index = startIndex;
        startIndex += sizeof(byte);
        return bytes[index];
    }

    public bool ToBool()
    {
        var index = startIndex;
        startIndex += sizeof(bool);
        return BitConverter.ToBoolean(bytes, index);
    }

    public UInt16 ToUInt16()
    {
        var index = startIndex;
        startIndex += sizeof(UInt16);
        return BitConverter.ToUInt16(bytes, index);
    }

    public UInt32 ToUInt32()
    {
        var index = startIndex;
        startIndex += sizeof(UInt32);
        return BitConverter.ToUInt32(bytes, index);
    }

    public UInt64 ToUInt64()
    {
        var index = startIndex;
        startIndex += sizeof(UInt64);
        return BitConverter.ToUInt64(bytes, index);
    }

    public Int16 ToInt16()
    {
        var index = startIndex;
        startIndex += sizeof(Int16);
        return BitConverter.ToInt16(bytes, index);
    }

    public Int32 ToInt32()
    {
        var index = startIndex;
        startIndex += sizeof(Int32);
        return BitConverter.ToInt32(bytes, index);
    }

    public Int64 ToInt64()
    {
        var index = startIndex;
        startIndex += sizeof(Int64);
        return BitConverter.ToInt64(bytes, index);
    }

    public float ToFloat()
    {
        var index = startIndex;
        startIndex += sizeof(float);
        return BitConverter.ToSingle(bytes, index);
    }

    public double ToDouble()
    {
        var index = startIndex;
        startIndex += sizeof(double);
        return BitConverter.ToDouble(bytes, index);
    }

    public string ToByteSizeString()
    {
        var count = ToByte();
        var index = startIndex;
        startIndex += count;
        return Encoding.UTF8.GetString(bytes, index, count);
    }

    public string ToUInt16SizeString()
    {
        var count = ToUInt16();
        var index = startIndex;
        startIndex += count;
        return Encoding.UTF8.GetString(bytes, index, count);
    }
}
