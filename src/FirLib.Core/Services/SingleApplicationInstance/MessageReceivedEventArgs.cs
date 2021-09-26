using System;
using System.Collections.Generic;
using System.Text;

namespace FirLib.Core.Services.SingleApplicationInstance
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public string Message { get; }

        public MessageReceivedEventArgs(string message)
        {
            this.Message = message;
        }
    }
}
