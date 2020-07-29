using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator.TestGui.Data
{
    public interface IMessageRecognizerAppSettings
    {
        public MessageRecognizerSettings CreateLibSettings();
    }
}
