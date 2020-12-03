using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator.TestGui.Data
{
    public interface IMessageRecognizerAppSettings
    {
        string Encoding { get; }

        MessageRecognizerSettings CreateLibSettings();
    }
}
