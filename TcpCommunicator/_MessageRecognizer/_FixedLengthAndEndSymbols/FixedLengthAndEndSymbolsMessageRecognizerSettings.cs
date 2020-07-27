using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunicator
{
    public class FixedLengthAndEndSymbolsMessageRecognizerSettings : MessageRecognizerSettings
    {
        public Encoding Encoding { get; set; }

        public string EndSymbols { get; set; }

        public int LengthIncludingEndSymbols { get; set; }
         
        public char FillSymbol { get; set; }

        public FixedLengthAndEndSymbolsMessageRecognizerSettings(Encoding encoding, string endSymbols, int lengthIncludingEndSymbols, char fillSymbol)
        {
            this.Encoding = encoding;
            this.EndSymbols = endSymbols;
            this.LengthIncludingEndSymbols = lengthIncludingEndSymbols;
            this.FillSymbol = fillSymbol;
        }

        /// <inheritdoc />
        public override MessageRecognizer CreateMessageRecognizer()
        {
            return new FixedLengthAndEndSymbolsMessageRecognizer(this.Encoding, this.EndSymbols, this.LengthIncludingEndSymbols, this.FillSymbol);
        }
    }
}
