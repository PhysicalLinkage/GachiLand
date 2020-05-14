using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Latte;
using UniRx;
using System.Text;

public class LoginTurn
{
    public Character Character { get; private set; }

    private LoginTurn(Character character) => Character = character;

    private static void Request(byte protocol, Action<byte[]> action)
    {
        var id = Host.Character.ID;
        var name = Host.Character.name;
        var modelName = Host.Character.Animator.avatar.name;
        var position = Host.Character.transform.position;
        ModelType model;

        if (Enum.TryParse<ModelType>(modelName, out model))
        {
            action(new Serializer()
                .AddBytes(protocol)
                .AddBytes(id)
                .AddByteSizeString(name)
                .AddBytes((UInt32)model)
                .AddBytes(position.x)
                .AddBytes(position.z)
                .GetBytes());
            /*
            var data = new List<byte> { protocol };
            data.AddRange(BitConverter.GetBytes(id));
            data.AddRange(BitConverter.GetBytes(name.Length));
            data.AddRange(Encoding.ASCII.GetBytes(name));
            data.AddRange(BitConverter.GetBytes((UInt32)model));
            data.AddRange(BitConverter.GetBytes(position.x));
            data.AddRange(BitConverter.GetBytes(position.z));
            action(data.ToArray());
            */
        }
    }

    public static void Request()
    {
        Request((byte)RoomTurnProtocol.LoginRequest, (data) => RoomTurn.Request(data));
    }

    private static void Reply(UInt16 destination)
    {
        Request((byte)TurnProtocol.LoginReply, (data) => Turn.Request(destination, data));
    }

    static Subject<LoginTurn> RequestSubject = new Subject<LoginTurn>();

    public static IObservable<LoginTurn> AsObservable => RequestSubject;

    private static void Receive(byte[] buffer, int headerSize, Action<UInt32, string, ModelType, Vector3> action)
    {
        var de      = new Deserializer(buffer, headerSize + sizeof(byte));
        var id      = de.ToUInt32();
        var name    = de.ToByteSizeString();
        var model   = (ModelType)de.ToInt32();
        var x       = de.ToFloat();
        var z       = de.ToFloat();

        /*
        UInt32 id       = BitConverter.ToUInt32             (buffer, headerSize + sizeof(byte));
        int nameLength  = BitConverter.ToInt32              (buffer, headerSize + sizeof(byte) + sizeof(Int32));
        string name     = Encoding.ASCII.GetString          (buffer, headerSize + sizeof(byte) + sizeof(Int32) * 2, nameLength);
        ModelType model = (ModelType)BitConverter.ToUInt32  (buffer, headerSize + sizeof(byte) + sizeof(Int32) * 2 + nameLength);
        float x         = BitConverter.ToSingle             (buffer, headerSize + sizeof(byte) + sizeof(Int32) * 3 + nameLength);
        float z         = BitConverter.ToSingle             (buffer, headerSize + sizeof(byte) + sizeof(Int32) * 3 + nameLength + sizeof(float));
        */

        action(id, name, model, new Vector3(x, 0f, z));
    }

    public static void RequestReceive(RoomTurn turn)
    {
        Receive(turn.Buffer, RoomTurn.HeaderSize, (id, name, model, position) =>
        {
            Reply(turn.Destination);
            Client.Load(turn.Destination, id, model, name, position);
        });
    }

    public static void ReplyReceive(Turn turn)
    {
        Receive(turn.Buffer, turn.HeaderSize, (id, name, model, position) =>
        {
            Client.Load(turn.Destination, id, model, name, position);
        });
    }
}
