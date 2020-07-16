using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TcpCommunicator.TestGui.Data
{
    public class MessageRecognizerEndSymbolSettings
    {
        private const string CATEGORY = "EndSymbol Recognizer";

        [Category(CATEGORY)]
        [EncodingWebName]
        public string Encoding { get; set; } = "utf-8";

        [Category(CATEGORY)]
        [TextAndHexadecimalEdit(nameof(Encoding))]
        public string EndSymbols { get; set; } = string.Empty;
    }
}
