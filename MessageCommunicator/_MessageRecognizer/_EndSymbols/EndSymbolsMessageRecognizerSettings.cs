using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator
{
    /// <summary>
    /// This class provides all settings for <see cref="EndSymbolsMessageRecognizer"/>.
    /// </summary>
    public class EndSymbolsMessageRecognizerSettings : MessageRecognizerSettings
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
        /// Creates a new <see cref="EndSymbolsMessageRecognizerSettings"/> instance.
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> to be used when convert characters to/from bytes.</param>
        /// <param name="endSymbols">The end symbols of received/sent messages.</param>
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
