using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MessageCommunicator.TestGui.Data
{
    [TypeAlias("MessageRecognizerEndSymbolSettings")]
    public class MessageRecognizerEndSymbolSettings : IMessageRecognizerAppSettings
    {
        private const string CATEGORY = "MessageRecognizer EndSymbol";

        [EncodingWebName]
        [Required]
        [Category(CATEGORY)]
        public string Encoding { get; set; } = "utf-8";

        [Category(CATEGORY)]
        [Required]
        [TextAndHexadecimalEdit(nameof(Encoding))]
        [HelpFileLink("ByteStreamHandler")]
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
