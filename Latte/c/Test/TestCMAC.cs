using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

public class TestCMAC : MonoBehaviour
{
    void Start()
    {
        byte[] key = new byte[CMAC.KEY_SIZE];
        byte[] msg = Encoding.ASCII.GetBytes("ok");
        byte[] mac;
    }
}
