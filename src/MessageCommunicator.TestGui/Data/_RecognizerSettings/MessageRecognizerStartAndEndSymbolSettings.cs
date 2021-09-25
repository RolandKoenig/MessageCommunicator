using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MessageCommunicator.TestGui.Data
{
    [TypeAlias("MessageRecognizerStartAndEndSymbolSettings")]
    public class MessageRecognizerStartAndEndSymbolSettings : IMessageRecognizerAppSettings
    {
        private const string CATEGORY = "MessageRecognizer StartAndEndSymbol";

        [EncodingWebName]
        [Required]
        [Category(CATEGORY)]
        [Description("Character encoding for communication.")]
        public string Encoding { get; set; } = "utf-8";

        [Category(CATEGORY)]
        [Required]
        [TextAndHexadecimalEdit(nameof(Encoding))]
        [Description("One or more characters which mark the start of a message.")]
        public string StartSymbols { get; set; } = string.Empty;

        [Category(CATEGORY)]
        [Required]
        [TextAndHexadecimalEdit(nameof(Encoding))]
        [Description("One or more characters which mark the end of a message.")]
        public string EndSymbols { get; set; } = string.Empty;

        /// <inheritdoc />
        public MessageRecognizerSettings CreateLibSettings()
        {
            return new StartAndEndSymbolsRecognizerSettings(
                System.Text.Encoding.GetEncoding(this.Encoding),
                this.StartSymbols, this.EndSymbols);
        }
    }
}
