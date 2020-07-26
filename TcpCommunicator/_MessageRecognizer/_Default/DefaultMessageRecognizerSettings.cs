using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunicator
{
    public class DefaultMessageRecognizerSettings : MessageRecognizerSettings
    {
        /// <inheritdoc />
        public override MessageRecognizer CreateMessageRecognizer(Encoding encoding)
        {
            return new DefaultMessageRecognizer(encoding);
        }
    }
}
