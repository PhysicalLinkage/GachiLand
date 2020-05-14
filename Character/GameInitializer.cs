using UnityEngine;
using Latte;
using UniRx;
using System;

public class GameInitializer : MonoBehaviour
{


    private void Awake()
    {
        Character.SubscribeAttacked();
        Host.Subscribe(this);
        Client.Subscribe();

        TurnServer.StartReceive(this);
    }

    private void Start()
    {
        Host.Load();
    }
}
