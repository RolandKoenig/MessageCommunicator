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

        [Category(CATEGORY)]
        [EncodingWebName]
        public string Encoding { get; set; } = "utf-8";
    }
}
