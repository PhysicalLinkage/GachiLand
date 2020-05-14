using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface UdpHeader
{
    byte[] Encryption(byte[] sendData);

    byte[] Decryption(byte[] receiveData);
}

public class RUDP : UdpHeader
{
    byte m_Sequence = byte.MaxValue;
    byte m_Ask = byte.MaxValue;
    byte m_PartnerAsk = byte.MaxValue;
    byte[][] m_Data = new byte[byte.MaxValue][];

    public byte[] Encryption(byte[] sendData)
    {
        m_Sequence++;
        m_Data[m_Sequence] = sendData;

        var data = new List<byte>() { m_Sequence, m_Ask };
        byte num = (byte)(m_Sequence - m_PartnerAsk);
        for (byte i = 0; i < num; i++)
        {
            data = data.Concat(m_Data[m_Sequence - i]).ToList();
        }
        return data.ToArray();
    }

    public byte[] Decryption(byte[] receiveData)
    {
        if (receiveData.Length < 3)
        {
            Debug.Log(receiveData.Length);
            return null;
        }

        m_Ask = receiveData[0];
        m_PartnerAsk = receiveData[1];
        return receiveData.Skip(2).ToArray();
    }
}
