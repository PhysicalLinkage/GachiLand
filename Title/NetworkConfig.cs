using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NetworkConfig : MonoBehaviour
{
    [SerializeField] string host = "192.168.112.39";
    [SerializeField] UInt16 port = 7777;

    public static string Host { get; private set; }
    public static UInt16 Port { get; private set; }

    private void Awake()
    {
        Host = host;
        Port = port;
    }
}
