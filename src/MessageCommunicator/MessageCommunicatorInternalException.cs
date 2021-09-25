using System;
using System.Collections.Generic;

namespace MessageCommunicator
{
    public class MessageCommunicatorInternalException : Exception
    {
        public MessageCommunicatorInternalException()
        {

        }

        public MessageCommunicatorInternalException(string message)
            : base(message)
        {

        }

        public MessageCommunicatorInternalException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
