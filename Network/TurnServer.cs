using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Latte;
using UniRx;
using System;

public enum TurnProtocol
{
    LoginReply,
}

public enum RoomTurnProtocol
{
    LoginRequest,
}

public static class TurnServer
{
    private static Dictionary<TurnProtocol, Action<Turn>> TurnReceives = new Dictionary<TurnProtocol, Action<Turn>>
    {
        { TurnProtocol.LoginReply, LoginTurn.ReplyReceive }
    };

    private static Dictionary<RoomTurnProtocol, Action<RoomTurn>> RoomTurnReceives = new Dictionary<RoomTurnProtocol, Action<RoomTurn>>
    {
        { RoomTurnProtocol.LoginRequest, LoginTurn.RequestReceive }
    };

    public static void StartReceive(MonoBehaviour mono)
    {
        Turn.AsObservable.Subscribe(turn =>
        {
            var protocol = (TurnProtocol)turn.Buffer[turn.HeaderSize];
            Action<Turn> action;
            if (TurnReceives.TryGetValue(protocol, out action))
            {
                action(turn);
            }
        }).AddTo(mono);

        

        RoomTurn.AsObservable.Subscribe(turn =>
        {
            var protocol = (RoomTurnProtocol)turn.Buffer[RoomTurn.HeaderSize];
            Action<RoomTurn> action;
            if (RoomTurnReceives.TryGetValue(protocol, out action))
            {
                action(turn);
            }
        }).AddTo(mono);
    }
}
