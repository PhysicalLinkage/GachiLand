using System;
using UniRx;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Message
{
    public byte[] data;
    public byte size;
}

public class ClientRUDPS
{
    public static int BYTES_MAC       => CMAC.MAC_SIZE;
    public static int BYTES_COUNTER   => sizeof(UInt32);
    public static int BYTES_ID        => sizeof(UInt16);
    public static int BYTES_SEQ       => sizeof(UInt16);
    public static int BYTES_ACK       => sizeof(UInt16);
    public static int BYTES_HEADER    => BYTES_MAC + BYTES_COUNTER + BYTES_ID + BYTES_SEQ + BYTES_ACK;

    class Header
    {
        public byte[] mac;
        public UInt32 counter;
        public UInt16 id;
        public UInt16 seq;
        public UInt16 ack;

        public void Init(byte[] data)
        {
            int mac_offset = BYTES_TYPE;
            int counter_offset = mac_offset + BYTES_MAC;
            int id_offset = counter_offset + BYTES_COUNTER;
            int seq_offset = id_offset + BYTES_ID;
            int ack_offset = seq_offset + BYTES_SEQ;

            mac = new byte[BYTES_MAC];
            Array.Copy(data, mac_offset, mac, 0, BYTES_MAC);
            counter = BitConverter.ToUInt32(data, counter_offset);
            id = BitConverter.ToUInt16(data, id_offset);
            seq = BitConverter.ToUInt16(data, seq_offset);
            ack = BitConverter.ToUInt16(data, ack_offset);
        }
    }

    private const int TYPE_CONTACT = 0;
    private const int TYPE_AUTH    = 1;
    private const int TYPE_MESSAGE = 2;
    private const int TYPE_LOGOUT  = 3;
    private const int TYPE_SIZE    = 4;

    private const int BYTES_TYPE    = 1;
    private const int BYTES_NONCE   = 12;
    
    private static int BYTES_CONTACT   => BYTES_TYPE + BYTES_NONCE + DH.KEY_SIZE;

    private ClientUDP ClientUDP { get; set; }

    private Subject<byte[]> ReceiveSubject { get; set; }

    private Action<byte[]> OnReceive;

    private byte[] public_key;
    private byte[] key;

    private Header header;
    private UInt16 prev_ack;

    private LinkedList<Message> SendMessages;

    private IDisposable ContactLoop;
    private IDisposable ConnectedLoop;

    public void Start(string host, int port)
    {
        ClientUDP = new ClientUDP(host, port);
        ReceiveSubject = new Subject<byte[]>();
        OnReceive = OnReceiveContact;
        header = new Header();
        prev_ack = header.ack = header.seq = 0;
        SendMessages = new LinkedList<Message>();

        ClientUDP.StartReceive();


        ClientUDP.ReceiveAsObservable.Subscribe(data => OnReceive(data));
        DH.RandSeed(System.Text.Encoding.ASCII.GetBytes("a;ijmaavoium389uvm02uvm@"));
        DH.New();
        DH.GenerateKey(out public_key);
        CMAC.New();



        ContactLoop = Observable.Interval(TimeSpan.FromSeconds(0.5)).Subscribe(l =>
        {
            if (l > 3)
            {
                Debug.Log("contact error");
                ContactLoop.Dispose();
                return;
            }

            Debug.Log("Contact-------");
            Contact();
        });


        ReceiveAsObservable.Subscribe(data =>
        {
            Debug.Log(System.Text.Encoding.ASCII.GetString(data));
        });
    }

    private void Contact()
    {
        var data = new byte[] { TYPE_CONTACT };
        ClientUDP.Send(data, data.Length);
    }

    public void Send(Message msg)
    {
        ++header.seq;
        SendMessages.AddLast(msg);

        var packet = new byte[ClientUDP.MTU];
        int i = 0;
        packet[i++] = TYPE_MESSAGE;
        int mac_offset = i;
        i += CMAC.MAC_SIZE;
        Array.Copy(BitConverter.GetBytes(header.counter), 0, packet, i, BYTES_COUNTER);
        i += BYTES_COUNTER;
        Array.Copy(BitConverter.GetBytes(header.id), 0, packet, i, BYTES_ID);
        i += BYTES_ID;
        Array.Copy(BitConverter.GetBytes(header.seq), 0, packet, i, BYTES_SEQ);
        i += BYTES_SEQ;
        Array.Copy(BitConverter.GetBytes(header.ack), 0, packet, i, BYTES_ACK);
        i += BYTES_ACK;

        CMAC.Init(key);
        CMAC.Update(packet, BYTES_TYPE + BYTES_MAC, BYTES_HEADER - BYTES_MAC);

        foreach (var message in SendMessages)
        {
            packet[i++] = message.size;
            Array.Copy(message.data, 0, packet, i, message.size);
            i += message.size;
            CMAC.Update(message.data, 0, message.size);
        }

        {
            byte[] mac_;
            CMAC.Final(out mac_);
            Array.Copy(mac_, 0, packet, mac_offset, BYTES_MAC);
            ClientUDP.Send(packet, i);
        }
    }

    public IObservable<byte[]> ReceiveAsObservable => ReceiveSubject;

    private void OnReceiveContact(byte[] data)
    {
        Debug.Log("contact ok");

        if (data.Length != BYTES_CONTACT)
        {
            return;
        }

        int key_offset = 0;
        int public_key_offset = BYTES_TYPE + BYTES_NONCE;
        DH.ComputeKey(out key, data, key_offset, public_key_offset);
        DH.Free();
        public_key.CopyTo(data, public_key_offset);
        data[0] = TYPE_AUTH;

        OnReceive = OnReceiveAuth;
        ContactLoop.Dispose();

        ConnectedLoop = Observable.Interval(TimeSpan.FromSeconds(0.5)).Subscribe(l =>
        {
            if (l > 3)
            {
                Debug.Log("auth error");
                ConnectedLoop.Dispose();
                return;
            }

            ClientUDP.Send(data, data.Length);
        });
    }

    private void OnReceiveAuth(byte[] data)
    {
        Debug.Log("auth ok");
        ConnectedLoop.Dispose();

        Debug.Log(data.Length);

        if (data.Length < BYTES_TYPE + BYTES_HEADER)
        {
            return;
        }

        Header recvHeader = new Header();
        recvHeader.Init(data);

        CMAC.Init(key);
        CMAC.Update(data, BYTES_TYPE + BYTES_MAC, BYTES_HEADER - BYTES_MAC);

        if (data.Length == BYTES_TYPE + BYTES_HEADER)
        {
            byte[] mac;
            CMAC.Final(out mac);

            if (!mac.SequenceEqual(recvHeader.mac))
            {
                return;
            }

            for (; prev_ack != recvHeader.ack; ++prev_ack)
            {
                if (SendMessages.Count > 0)
                {
                    SendMessages.RemoveFirst();
                }
            }

            ConnectedLoop.Dispose();

            header.id = recvHeader.id;
            OnReceive = OnReceiveMessage;
            Debug.Log("connected ok");

            Observable.Interval(TimeSpan.FromMilliseconds(1000)).Subscribe(_ =>
            {
                var packet = new byte[ClientUDP.MTU];
                int i = 0;
                packet[i++] = TYPE_MESSAGE;
                int mac_offset = i;
                i += CMAC.MAC_SIZE;
                Array.Copy(BitConverter.GetBytes(header.counter), 0, packet, i, BYTES_COUNTER);
                i += BYTES_COUNTER;
                Array.Copy(BitConverter.GetBytes(header.id), 0, packet, i, BYTES_ID);
                i += BYTES_ID;
                Array.Copy(BitConverter.GetBytes(header.seq), 0, packet, i, BYTES_SEQ);
                i += BYTES_SEQ;
                Array.Copy(BitConverter.GetBytes(header.ack), 0, packet, i, BYTES_ACK);
                i += BYTES_ACK;

                CMAC.Init(key);
                CMAC.Update(packet, BYTES_TYPE + BYTES_MAC, BYTES_HEADER - BYTES_MAC);

                foreach (var message in SendMessages)
                {
                    packet[i++] = message.size;
                    Array.Copy(message.data, 0, packet, i, message.size);
                    i += message.size;
                    CMAC.Update(message.data, 0, message.size);
                }

                {
                    byte[] mac_;
                    CMAC.Final(out mac_);
                    Array.Copy(mac_, 0, packet, mac_offset, BYTES_MAC);
                    ClientUDP.Send(packet, i);
                }

            });
        }
    }

    private void OnReceiveMessage(byte[] data)
    {
        if (data.Length < BYTES_TYPE + BYTES_HEADER)
        {
            return;
        }

        Header recvHeader = new Header();
        recvHeader.Init(data);

        CMAC.Init(key);
        CMAC.Update(data, BYTES_TYPE + BYTES_MAC, BYTES_HEADER - BYTES_MAC);

        if (data.Length == BYTES_TYPE + BYTES_HEADER)
        {
            byte[] mac;
            CMAC.Final(out mac);

            if (!mac.SequenceEqual(recvHeader.mac))
            {
                Debug.Log("r1");
                return;
            }
        }
        else if (data.Length > BYTES_TYPE + BYTES_HEADER)
        {
            var iovs = new List<(int, int)>();
            for (int offset = BYTES_TYPE + BYTES_HEADER; offset < data.Length; )
            {
                byte seqLen = data[offset++];
                int seqSize = offset + seqLen;
                if (data.Length == seqSize)
                {
                    iovs.Add((offset, seqLen));
                    CMAC.Update(data, offset, seqLen);
                    offset += seqLen;
                    break;
                }
                else if (data.Length > seqSize)
                {
                    iovs.Add((offset, seqLen));
                    CMAC.Update(data, offset, seqLen);
                    offset += seqLen;
                }
                else
                {
                    Debug.Log("r2");
                    return;
                }
            }

            byte[] mac;
            CMAC.Final(out mac);
            if (!mac.SequenceEqual(recvHeader.mac))
            {
                Debug.Log("r3");
                return;
            }

            int distance = recvHeader.seq - header.ack;
            distance = (distance >= 0) ? distance : ushort.MaxValue - distance + 1;

            if (distance > UInt16.MaxValue / 2)
            {
                return;
            }

            if (distance != 0)
            {
                for (int i = 0; i < distance; ++i)
                {
                    var iov = iovs[iovs.Count - 1 - i];
                    byte[] sm = new byte[iov.Item2];
                    Array.Copy(data, iov.Item1, sm, 0, iov.Item2);
                    ReceiveSubject.OnNext(sm);
                }

                header.ack = recvHeader.seq;
            }

        }

        for (; prev_ack != recvHeader.ack; ++prev_ack)
        {
            SendMessages.RemoveFirst();
        }
    }
}
