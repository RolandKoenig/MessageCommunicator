using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirLib.Core
{
    internal class Win32WindowHandleWrapper : System.Windows.Forms.IWin32Window
    {
        private readonly System.IntPtr _handle;

        public Win32WindowHandleWrapper(System.IntPtr handle)
        {
            _handle = handle;
        }

        System.IntPtr System.Windows.Forms.IWin32Window.Handle
        {
            get { return _handle; }
        }
    }
}
