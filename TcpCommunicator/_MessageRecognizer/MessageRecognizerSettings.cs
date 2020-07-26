using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunicator
{
    public abstract class MessageRecognizerSettings
    {
        public abstract MessageRecognizer CreateMessageRecognizer(ByteStreamHandler byteStreamHandler, Encoding encoding);
    }
}
