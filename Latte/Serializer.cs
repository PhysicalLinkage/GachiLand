using System;
using System.Text;
using System.Collections.Generic;

public class Serializer
{
    List<byte> bytes;

    public Serializer(int capacity = 2000)
    {
        bytes = new List<byte>(capacity);
    }
    public byte[] GetBytes()
    {
        return bytes.ToArray();
    }

    public Serializer AddBytes(byte x)
    {
        bytes.Add(x);
        return this;
    }

    public Serializer AddBytes(bool x)
    {
        bytes.AddRange(BitConverter.GetBytes(x));
        return this;
    }

    public Serializer AddBytes(UInt16 x)
    {
        bytes.AddRange(BitConverter.GetBytes(x));
        return this;
    }

    public Serializer AddBytes(UInt32 x)
    {
        bytes.AddRange(BitConverter.GetBytes(x));
        return this;
    }

    public Serializer AddBytes(UInt64 x)
    {
        bytes.AddRange(BitConverter.GetBytes(x));
        return this;
    }

    public Serializer AddBytes(Int16 x)
    {
        bytes.AddRange(BitConverter.GetBytes(x));
        return this;
    }

    public Serializer AddBytes(Int32 x)
    {
        bytes.AddRange(BitConverter.GetBytes(x));
        return this;
    }

    public Serializer AddBytes(Int64 x)
    {
        bytes.AddRange(BitConverter.GetBytes(x));
        return this;
    }

    public Serializer AddBytes(float x)
    {
        bytes.AddRange(BitConverter.GetBytes(x));
        return this;
    }

    public Serializer AddBytes(double x)
    {
        bytes.AddRange(BitConverter.GetBytes(x));
        return this;
    }

    public Serializer AddByteSizeString(string x)
    {
        var bs = Encoding.UTF8.GetBytes(x);
        bytes.Add((byte)bs.Length);
        bytes.AddRange(bs);
        return this;
    }

    public Serializer AddUInt16SizeString(string x)
    {
        var bs = Encoding.UTF8.GetBytes(x);
        bytes.AddRange(BitConverter.GetBytes((UInt16)bs.Length));
        bytes.AddRange(bs);
        return this;
    }
}
