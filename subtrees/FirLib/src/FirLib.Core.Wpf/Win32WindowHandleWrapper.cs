using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirLib.Core;

internal class Win32WindowHandleWrapper : IWin32Window
{
    private readonly IntPtr _handle;

    public Win32WindowHandleWrapper(IntPtr handle)
    {
        _handle = handle;
    }

    IntPtr IWin32Window.Handle
    {
        get { return _handle; }
    }
}