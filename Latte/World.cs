using System.Collections.Generic;
using System;
using UniRx;

namespace Latte
{
    public static class World
    {
        public static UInt16 Size { get; private set; }
        public static UInt16[] Numbers { get; private set; }

        static Subject<Unit> Subject = new Subject<Unit>();

        public static IObservable<Unit> AsObservable => Subject;

        public static void Receive(byte[] buffer)
        {
            var world_size = BitConverter.ToUInt16(buffer, sizeof(byte));
            var world_number = BitConverter.ToUInt16(buffer, sizeof(byte) + sizeof(UInt16));
            var numbers = new List<UInt16>();
            for (int index = 0; index < world_number; ++index)
            {
                numbers.Add(BitConverter.ToUInt16(buffer, sizeof(byte) + sizeof(UInt16) * (2 + index)));
            }
            Size = world_size;
            Numbers = numbers.ToArray();
            Subject.OnNext(Unit.Default);
        }

        public static void Request()
        {
            Server.Send(new byte[] { (byte)Server.Protocol.WorldType });
        }
    }
}
