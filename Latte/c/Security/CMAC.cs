using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class CMAC
{
    [DllImport("libcrypto1")]
    static extern int cmac_key_size();

    [DllImport("libcrypto1")]
    static extern int cmac_mac_size();

    [DllImport("libcrypto1")]
    static extern void cmac_new();

    [DllImport("libcrypto1")]
    static extern int cmac_init(IntPtr key);

    [DllImport("libcrypto1")]
    static extern int cmac_update(IntPtr message, int size);

    [DllImport("libcrypto1")]
    static extern int cmac_final(out IntPtr mac);

    [DllImport("libcrypto1")]
    static extern void cmac_free();

    static public int KEY_SIZE => cmac_key_size();
    static public int MAC_SIZE => cmac_mac_size();

    public static void New()
    {
        cmac_new();
    }

    public static void Free()
    {
        cmac_free();
    }

    public static bool Init(byte[] key)
    {
        using (var disposableKey = new DisposablePtr(KEY_SIZE))
        {
            Marshal.Copy(key, 0, disposableKey.ptr, KEY_SIZE);
            return cmac_init(disposableKey.ptr) == 1;
        }
    }

    public static bool Update(byte[] msg, int offset, int length)
    {
        using (var disposableMsg = new DisposablePtr(length))
        {
            Marshal.Copy(msg, offset, disposableMsg.ptr, length);
            return cmac_update(disposableMsg.ptr, length) == 1;
        }
    }

    public static bool Final(out byte[] mac)
    {
        mac = new byte[MAC_SIZE];
        using (var disposableMac = new DisposablePtr(MAC_SIZE))
        {
            if (cmac_final(out disposableMac.ptr) == 1)
            {
                Marshal.Copy(disposableMac.ptr, mac, 0, MAC_SIZE);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

