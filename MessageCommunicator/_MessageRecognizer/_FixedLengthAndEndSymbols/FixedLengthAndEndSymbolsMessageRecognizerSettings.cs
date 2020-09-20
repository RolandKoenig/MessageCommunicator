using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator
{
    /// <summary>
    /// This class provides all settings for <see cref="FixedLengthAndEndSymbolsMessageRecognizer"/>.
    /// </summary>
    public class FixedLengthAndEndSymbolsMessageRecognizerSettings : MessageRecognizerSettings
    {
        /// <summary>
        /// Gets or sets the <see cref="Encoding"/> to be used when convert characters to/from bytes.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Gets or sets the end symbols of received/sent messages.
        /// </summary>
        public string EndSymbols { get; set; }

        /// <summary>
        /// Gets or sets the total length of received/sent messages.
        /// </summary>
        public int LengthIncludingEndSymbols { get; set; }
         
        /// <summary>
        /// Gets or sets the fill symbol for messages shorter than the fixed length.
        /// </summary>
        public char FillSymbol { get; set; }

        /// <summary>
        /// Creates a new <see cref="FixedLengthAndEndSymbolsMessageRecognizerSettings"/> instance.
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> to be used when convert characters to/from bytes.</param>
        /// <param name="endSymbols">The end symbols of received/sent messages.</param>
        /// <param name="lengthIncludingEndSymbols">Total length of received/sent messages.</param>
        /// <param name="fillSymbol">Fill symbol for messages shorter than the fixed length.</param>
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
