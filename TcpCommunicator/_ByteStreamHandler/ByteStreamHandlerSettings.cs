using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunicator
{
    public abstract class ByteStreamHandlerSettings
    {
        public abstract ByteStreamHandler CreateByteStreamHandler();
    }
}
