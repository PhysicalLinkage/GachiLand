using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UniRx.Async;
using UniRx;
using System;
using UnityEngine;

namespace Latte
{
    public static class Server
    {
        public enum Protocol
        {
            WorldType = 0,
            RoomType = 1,
            LoginType = 2,
            LogoutType = 3,
            TurnType = 4,
            RoomTurnType = 5
        }

        public static ClientRUDPS ClientRUDPS { get; } = new ClientRUDPS();

        public static Dictionary<Protocol, Action<byte[]>> OnNexts = new Dictionary<Protocol, Action<byte[]>>
        {
            { Protocol.WorldType, World.Receive },
            { Protocol.RoomType, Room.Receive },
            { Protocol.LoginType, Login.Receive },
            { Protocol.LogoutType, Logout.Receive },
            { Protocol.TurnType, Turn.Receive },
            { Protocol.RoomTurnType, RoomTurn.Receive }
        };

        public static bool IsRunning { get; private set; } = false;

        public static void Receive()
        {
            ClientRUDPS.Start("13.231.248.183", 55488);

            ClientRUDPS.ReceiveAsObservable.Subscribe(buffer =>
            {
                // Debug.Log(buffer.Length);
                var type = (Protocol)buffer[0];
                Action<byte[]> action;
                if (OnNexts.TryGetValue(type, out action))
                {
                    action(buffer);
                }
            });
        }

        public static void StopReceive() => IsRunning = false;

        public static void Send(byte[] data)
        {
            var msg = new Message();
            msg.data = data;
            msg.size = (byte)data.Length;
            ClientRUDPS.Send(msg);
        }
    }
}
