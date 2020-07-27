using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunicator
{
    public class EndSymbolMessageRecognizerSettings : MessageRecognizerSettings
    {
        public Encoding Encoding { get; set; }

        public string EndSymbols { get; set; }

        public EndSymbolMessageRecognizerSettings(Encoding encoding, string endSymbols)
        {
            this.Encoding = encoding;
            this.EndSymbols = endSymbols;
        }

        /// <inheritdoc />
        public override MessageRecognizer CreateMessageRecognizer()
        {
            return new EndSymbolMessageRecognizer(this.Encoding, this.EndSymbols);
        }
    }
}
