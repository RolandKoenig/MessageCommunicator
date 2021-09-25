using System;
using System.Collections.Generic;

namespace MessageCommunicator
{
    /// <summary>
    /// This exception type is thrown during message recognition in an <see cref="IMessageRecognizer"/> implementation.
    /// </summary>
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
