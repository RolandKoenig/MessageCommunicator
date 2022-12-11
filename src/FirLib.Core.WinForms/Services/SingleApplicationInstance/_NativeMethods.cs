using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FirLib.Core.Services.SingleApplicationInstance;

internal static class NativeMethods
{
    public const int WM_COPYDATA = 0x4A;

    [DllImport("user32", SetLastError = true)]
    public static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

    [DllImport("user32.dll", CharSet=CharSet.Auto)]
    public static extern IntPtr SendMessage(IntPtr handle, int msg, int param, ref CopyDataStruct copyData);
}