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
        [DisplayName("EndSymbol Encoding")]
        public string EndSymbolEncoding { get; set; } = "utf-8";

        [Category(CATEGORY)]
        [TextAndHexadecimalEdit(nameof(EndSymbolEncoding))]
        public string EndSymbols { get; set; } = string.Empty;
    }
}
