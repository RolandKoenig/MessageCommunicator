using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator
{
    public class StartAndEndSymbolsRecognizerSettings : MessageRecognizerSettings
    {
        public Encoding Encoding { get; set; }

        public string StartSymbols { get; set; }

        public string EndSymbols { get; set; }

        public StartAndEndSymbolsRecognizerSettings(Encoding encoding, string startSymbols, string endSymbols)
        {
            this.Encoding = encoding;
            this.StartSymbols = startSymbols;
            this.EndSymbols = endSymbols;
        }

        /// <inheritdoc />
        public override MessageRecognizer CreateMessageRecognizer()
        {
            return new StartAndEndSymbolsRecognizer(this.Encoding, this.StartSymbols, this.EndSymbols);
        }
    }
}
