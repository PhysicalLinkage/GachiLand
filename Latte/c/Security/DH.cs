using System;
using System.Runtime.InteropServices;

public static class DH
{
    [DllImport("libcrypto1")]
    static extern void dh_rand_seed(IntPtr seed, uint size);

    [DllImport("libcrypto1")]
    static extern int dh_key_size();

    [DllImport("libcrypto1")]
    static extern int dh_new();

    [DllImport("libcrypto1")]
    static extern int dh_generate_key(out IntPtr public_key);

    [DllImport("libcrypto1")]
    static extern void dh_compute_key(out IntPtr key, IntPtr remote_public_key);

    [DllImport("libcrypto1")]
    static extern void dh_free();

    public static int KEY_SIZE => dh_key_size();

    public static void RandSeed(byte[] seed)
    {
        using (var disposableSeed = new DisposablePtr(KEY_SIZE))
        {
            Marshal.Copy(seed, 0, disposableSeed.ptr, seed.Length);
            dh_rand_seed(disposableSeed.ptr, (uint)seed.Length);
        }
    }

    public static bool New()
    {
        return dh_new() == 1;
    }

    public static bool GenerateKey(out byte[] public_key)
    {
        public_key = new byte[KEY_SIZE];
        using (var disposablePublicKey = new DisposablePtr(KEY_SIZE))
        {
            if (dh_generate_key(out disposablePublicKey.ptr) != 0)
            {
                Marshal.Copy(disposablePublicKey.ptr, public_key, 0, KEY_SIZE);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public static void ComputeKey(out byte[] key, byte[] remote_public_key
        , int key_offset = 0, int remote_public_key_offset = 0)
    {
        key = new byte[KEY_SIZE];
        using (var disposableKey = new DisposablePtr(KEY_SIZE))
        using (var disposableRemotePublicKey = new DisposablePtr(KEY_SIZE))
        {
            Marshal.Copy(remote_public_key, remote_public_key_offset, disposableRemotePublicKey.ptr, KEY_SIZE);
            dh_compute_key(out disposableKey.ptr, disposableRemotePublicKey.ptr);
            Marshal.Copy(disposableKey.ptr, key, key_offset, KEY_SIZE);
        }
    }

    public static void Free()
    {
        dh_free();
    }
}
