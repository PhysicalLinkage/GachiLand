using System.Net;
using System.Net.Sockets;
using System;
using UniRx;
using UniRx.Async;
using UnityEngine;

public class ClientUDP
{
    public const int MTU = 512;

    public bool IsRunning { get; private set; }

    private UdpClient UdpClient { get; }

    private Subject<byte[]> ReceiveSubject  { get; }
    

    public ClientUDP(string host, int port)
    {
        IsRunning = false;
        UdpClient       = new UdpClient(host, port);
        ReceiveSubject  = new Subject<byte[]>();
    }

    public IObservable<byte[]> ReceiveAsObservable => ReceiveSubject;

    private async UniTask ReceiveAsync()
    {
        if (IsRunning)
        {
            return;
        }

        IsRunning = true;

        while (IsRunning)
        {
            IPEndPoint _ = new IPEndPoint(IPAddress.Any, 0);
            var bytes = await UniTask.Run(() => UdpClient.Receive(ref _));
            ReceiveSubject.OnNext(bytes);
        }
    }

    public void StartReceive()
    {
        var _ = ReceiveAsync();
    }

    public void StopReceive()
    {
        IsRunning = false;
    }

    public void Send(byte[] data, int size)
    {
        var _ = UniTask.Run(() => UdpClient.Send(data, size));
    }
}
