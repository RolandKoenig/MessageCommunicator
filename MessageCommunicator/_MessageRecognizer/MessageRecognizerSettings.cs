using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator
{
    public abstract class MessageRecognizerSettings
    {
        public abstract MessageRecognizer CreateMessageRecognizer();
    }
}
