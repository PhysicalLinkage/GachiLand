using System;
using UnityEngine;

public class TestDH : MonoBehaviour
{
    private void Start()
    {
        byte[] seed = System.Text.Encoding.ASCII.GetBytes("38pyb2p8yn2ybauynf874t");
        byte[] remote_public_key;
        byte[] public_key;
        byte[] key;

        DH.RandSeed(seed);

        DH.New();

        DH.GenerateKey(out remote_public_key);

        DH.Free();

        DH.New();

        DH.GenerateKey(out public_key);
        DH.ComputeKey(out key, remote_public_key);

        DH.Free();

        Debug.Log($"remote_public_key : {BitConverter.ToString(remote_public_key)}");
        Debug.Log($"public_key : {BitConverter.ToString(public_key)}");
        Debug.Log($"key : {BitConverter.ToString(key)}");
    }
}
