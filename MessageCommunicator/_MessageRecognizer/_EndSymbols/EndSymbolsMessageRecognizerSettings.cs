using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator
{
    public class EndSymbolsMessageRecognizerSettings : MessageRecognizerSettings
    {
        public Encoding Encoding { get; set; }

        public string EndSymbols { get; set; }

        public EndSymbolsMessageRecognizerSettings(Encoding encoding, string endSymbols)
        {
            this.Encoding = encoding;
            this.EndSymbols = endSymbols;
        }

        /// <inheritdoc />
        public override MessageRecognizer CreateMessageRecognizer()
        {
            return new EndSymbolsMessageRecognizer(this.Encoding, this.EndSymbols);
        }
    }
}
