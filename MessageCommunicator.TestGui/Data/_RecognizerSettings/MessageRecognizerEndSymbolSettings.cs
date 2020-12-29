using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MessageCommunicator.TestGui.Data
{
    [TypeAlias("MessageRecognizerEndSymbolSettings")]
    public class MessageRecognizerEndSymbolSettings : IMessageRecognizerAppSettings
    {
        private const string CATEGORY = "MessageRecognizer EndSymbol";

        [EncodingWebName]
        [Required]
        [Category(CATEGORY)]
        [Description("Character encoding for communication.")]
        public string Encoding { get; set; } = "utf-8";

        [Category(CATEGORY)]
        [Required]
        [TextAndHexadecimalEdit(nameof(Encoding))]
        [Description("One or more characters which mark the end of a message.")]
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
