using System;
using System.Collections.Generic;
using System.Text;
using UniRx;

namespace Latte
{
    public class RoomTurn
    {
        public byte[] Buffer { get; private set; }
        public UInt16 Destination => BitConverter.ToUInt16(Buffer, sizeof(byte));
        public string Message => Encoding.UTF8.GetString(Buffer, HeaderSize, Buffer.Length - HeaderSize);

        private RoomTurn(byte[] buffer)
        {
            Buffer = buffer;
        }

        public static int HeaderSize => sizeof(byte) + sizeof(UInt16) * 4;

        static Subject<RoomTurn> Subject = new Subject<RoomTurn>();

        public static IObservable<RoomTurn> AsObservable => Subject;

        public static void Receive(byte[] buffer)
        {
            Subject.OnNext(new RoomTurn(buffer));
        }

        public static void Request(byte[] message)
        {
            if (Login.IsSuccess)
            {
                var data = new List<byte> { (byte)Server.Protocol.RoomTurnType };
                data.AddRange(BitConverter.GetBytes(Login.ID));
                data.AddRange(BitConverter.GetBytes(Login.Passwords));
                data.AddRange(BitConverter.GetBytes(Login.WorldIndex));
                data.AddRange(BitConverter.GetBytes(Login.RoomIndex));
                data.AddRange(message);
                Server.Send(data.ToArray());
            }
        }

        public static void Request(string message)
        {
            Request(Encoding.UTF8.GetBytes(message));
        }
    }
}
