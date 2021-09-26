using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace FirLib.Core.Services.SingleApplicationInstance
{
    internal class CopyDataControl : NativeWindow
    {
        public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

        public static void Send(IntPtr target, string message)
        {
            var rawData = Encoding.UTF8.GetBytes(message);
            var dataPointer = Marshal.AllocCoTaskMem(rawData.Length);
            try
            {
                Marshal.Copy(rawData, 0, dataPointer, rawData.Length);

                var copyData = new CopyDataStruct();
                copyData.DataType = IntPtr.Zero;
                copyData.DataSize = rawData.Length;
                copyData.DataPointer = dataPointer;

                NativeMethods.SendMessage(target, NativeMethods.WM_COPYDATA, 0, ref copyData);
            }
            finally
            {
                Marshal.FreeCoTaskMem(dataPointer);
            }
        }

        protected override void WndProc(ref Message message)
        {
            switch (message.Msg)
            {
                case NativeMethods.WM_COPYDATA:
                    var copyData = Marshal.PtrToStructure<CopyDataStruct>(message.LParam);

                    var rawData = Array.Empty<byte>();
                    if (copyData.DataSize > 0)
                    {
                        rawData = new byte[copyData.DataSize];
                        Marshal.Copy(
                            copyData.DataPointer,
                            rawData,
                            0,
                            copyData.DataSize);
                    }

                    message.Result = (IntPtr)1;
                    this.MessageReceived?.Invoke(this, new MessageReceivedEventArgs(
                        Encoding.UTF8.GetString(rawData)));
                    break;
            }

            base.WndProc(ref message);
        }
    }
}
