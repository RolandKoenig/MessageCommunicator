using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator
{
    /// <summary>
    /// This class provides all settings for <see cref="StartAndEndSymbolsRecognizer"/>.
    /// </summary>
    public class StartAndEndSymbolsRecognizerSettings : MessageRecognizerSettings
    {
        /// <summary>
        /// Gets or sets the <see cref="Encoding"/> to be used when convert characters to/from bytes.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Gets or sets the start symbols of received/sent messages.
        /// </summary>
        public string StartSymbols { get; set; }

        /// <summary>
        /// Gets or sets the end symbols of received/sent messages.
        /// </summary>
        public string EndSymbols { get; set; }

        /// <summary>
        /// Creates a new <see cref="StartAndEndSymbolsRecognizerSettings"/> instance.
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> to be used when convert characters to/from bytes.</param>
        /// <param name="startSymbols">The start symbols of received/sent messages.</param>
        /// <param name="endSymbols">The end symbols of received/sent messages.</param>
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
