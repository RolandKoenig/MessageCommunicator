using System;
using System.Runtime.InteropServices;

namespace FirLib.Core.Services.SingleApplicationInstance
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CopyDataStruct
    {
        internal IntPtr DataType;
        internal int DataSize;
        internal IntPtr DataPointer;
    }
}
