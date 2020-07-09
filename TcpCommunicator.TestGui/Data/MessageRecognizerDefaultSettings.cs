using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TcpCommunicator.TestGui.Data
{
    public class MessageRecognizerDefaultSettings
    {
        private const string CATEGORY = "Default Recognizer";

        [Category(CATEGORY)]
        public string Encoding { get; set; } = string.Empty;
    }
}
