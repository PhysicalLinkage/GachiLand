using System.Collections.Generic;
using System;
using UniRx;

namespace Latte
{
    public static class Room
    {
        public static UInt16 WorldIndex { get; private set; }
        public static UInt16 Size { get; private set; }
        public static UInt16[] Numbers { get; private set; }

        static Subject<Unit> Subject = new Subject<Unit>();

        public static IObservable<Unit> AsObservable => Subject;

        public static void Receive(byte[] buffer)
        {
            var world_index = BitConverter.ToUInt16(buffer, sizeof(byte));
            var room_size = BitConverter.ToUInt16(buffer, sizeof(byte) + sizeof(UInt16));
            var room_number = BitConverter.ToUInt16(buffer, sizeof(byte) + sizeof(UInt16) * 2);
            var numbers = new List<UInt16>();
            for (int room_index = 0; room_index < room_number; ++room_index)
            {
                numbers.Add(BitConverter.ToUInt16(buffer, sizeof(byte) + sizeof(UInt16) * (3 + room_index)));
            }
            WorldIndex = world_index;
            Size = room_size;
            Numbers = numbers.ToArray();
            Subject.OnNext(Unit.Default);
        }

        public static void Request(UInt16 world_index)
        {
            var data = new List<byte> { (byte)Server.Protocol.RoomType };
            data.AddRange(BitConverter.GetBytes(world_index));
            Server.Send(data.ToArray());
        }
    }
}
