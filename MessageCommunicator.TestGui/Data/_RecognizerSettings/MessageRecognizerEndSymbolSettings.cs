using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MessageCommunicator.TestGui.Data
{
    public class MessageRecognizerEndSymbolSettings : IMessageRecognizerAppSettings
    {
        private const string CATEGORY = "EndSymbol Recognizer";

        [EncodingWebName]
        [Category(CATEGORY)]
        public string Encoding { get; set; } = "utf-8";

        [Category(CATEGORY)]
        [TextAndHexadecimalEdit(nameof(Encoding))]
        public string EndSymbols { get; set; } = string.Empty;

        /// <inheritdoc />
        public MessageRecognizerSettings CreateLibSettings()
        {
            return new EndSymbolsMessageRecognizerSettings(
                System.Text.Encoding.GetEncoding(this.Encoding),
                this.EndSymbols);
        }
    }
}
