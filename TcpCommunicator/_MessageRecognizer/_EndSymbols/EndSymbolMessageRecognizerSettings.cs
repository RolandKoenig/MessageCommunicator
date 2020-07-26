using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunicator
{
    public class EndSymbolMessageRecognizerSettings : MessageRecognizerSettings
    {
        public string EndSymbols { get; set; }

        public EndSymbolMessageRecognizerSettings(string endSymbols)
        {
            this.EndSymbols = endSymbols;
        }

        /// <inheritdoc />
        public override MessageRecognizer CreateMessageRecognizer(Encoding encoding)
        {
            return new EndSymbolMessageRecognizer(encoding, this.EndSymbols);
        }
    }
}
