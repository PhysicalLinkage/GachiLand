using System;
using System.Collections.Generic;
using UniRx;
using System.Text;

namespace Latte
{
    public class Turn
    {
        public byte[] Buffer { get; private set; }
        public UInt16 Destination => BitConverter.ToUInt16(Buffer, sizeof(byte));
        public string Message => Encoding.ASCII.GetString(Buffer, HeaderSize, Buffer.Length - HeaderSize);

        private Turn(byte[] buffer)
        {
            Buffer = buffer;
        }

        public int HeaderSize => sizeof(byte) + sizeof(UInt16) * 3;

        static Subject<Turn> Subject = new Subject<Turn>();

        public static IObservable<Turn> AsObservable => Subject;

        public static void Receive(byte[] buffer)
        {
            Subject.OnNext(new Turn(buffer));
        }

        public static void Request(UInt16 destination, byte[] message)
        {
            if (Login.IsSuccess)
            {
                var data = new List<byte> { (byte)Server.Protocol.TurnType };
                data.AddRange(BitConverter.GetBytes(Login.ID));
                data.AddRange(BitConverter.GetBytes(Login.Passwords));
                data.AddRange(BitConverter.GetBytes(destination));
                data.AddRange(message);
                Server.Send(data.ToArray());
            }
        }

        public static void Request(UInt16 destination, string message)
        {
            Request(destination, Encoding.ASCII.GetBytes(message));
        }
    }
}
