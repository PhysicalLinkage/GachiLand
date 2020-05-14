using System;
using System.Runtime.InteropServices;

class DisposablePtr : IDisposable
{
    public IntPtr ptr;

    public DisposablePtr(int cb)
    {
        ptr = Marshal.AllocCoTaskMem(cb);
    }

    public void Dispose()
    {
        Marshal.FreeCoTaskMem(ptr);
    }
}

