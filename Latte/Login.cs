using System;
using System.Collections.Generic;
using UniRx;

namespace Latte
{
    public static class Login
    {
        public static UInt16 WorldIndex { get; private set; }
        public static UInt16 RoomIndex { get; private set; }
        public static UInt16 NumberIndex { get; private set; }
        public static bool IsSuccess { get; private set; }
        public static UInt16 ID { get; private set; }
        public static UInt16 Passwords { get; private set; }

        static Subject<Unit> Subject = new Subject<Unit>();

        public static IObservable<Unit> AsObservable => Subject;

        public static void Receive(byte[] buffer)
        {
            WorldIndex = BitConverter.ToUInt16(buffer, sizeof(byte));
            RoomIndex = BitConverter.ToUInt16(buffer, sizeof(byte) + sizeof(UInt16));
            NumberIndex = BitConverter.ToUInt16(buffer, sizeof(byte) + sizeof(UInt16) * 2);
            IsSuccess = BitConverter.ToBoolean(buffer, sizeof(byte) + sizeof(UInt16) * 3);
            ID = BitConverter.ToUInt16(buffer, sizeof(byte) * 2 + sizeof(UInt16) * 3);
            Passwords = BitConverter.ToUInt16(buffer, sizeof(byte) * 2 + sizeof(UInt16) * 4);
            Subject.OnNext(Unit.Default);
        }

        public static void Request(UInt16 world_index, UInt16 room_index)
        {
            var data = new List<byte> { (byte)Server.Protocol.LoginType };
            data.AddRange(BitConverter.GetBytes(world_index));
            data.AddRange(BitConverter.GetBytes(room_index));
            Server.Send(data.ToArray());
        }
    }
}
