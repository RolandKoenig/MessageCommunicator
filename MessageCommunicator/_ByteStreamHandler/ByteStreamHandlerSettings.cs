using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator
{
    public abstract class ByteStreamHandlerSettings
    {
        public abstract ByteStreamHandler CreateByteStreamHandler();
    }
}
