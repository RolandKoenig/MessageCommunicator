using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;

namespace TcpCommunicator.TestGui.Data
{
    public class MessageRecognizerDefaultSettings
    {
        private const string CATEGORY = "Default Recognizer";

        [EncodingWebName]
        [Category(CATEGORY)]
        public string Encoding { get; set; } = "utf-8";
    }
}
