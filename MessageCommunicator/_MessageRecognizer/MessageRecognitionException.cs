using System;
using System.Collections.Generic;

namespace MessageCommunicator
{
    public class MessageRecognitionException : Exception
    {
        public MessageRecognitionException()
        {

        }

        public MessageRecognitionException(string message)
            : base(message)
        {

        }

        public MessageRecognitionException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
