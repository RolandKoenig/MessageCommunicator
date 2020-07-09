using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TcpCommunicator.TestGui.Data
{
    public class MessageRecognizerEndSymbolSettings
    {
        private const string CATEGORY = "EndSymbol Recognizer";

        [Category(CATEGORY)]
        public string Encoding { get; set; } = string.Empty;

        [Category(CATEGORY)]
        public string EndSymbols { get; set; } = string.Empty;
    }
}
