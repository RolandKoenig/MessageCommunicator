using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MessageCommunicator.TestGui.Data
{
    [TypeAlias("MessageRecognizerStartAndEndSymbolSettings")]
    public class MessageRecognizerStartAndEndSymbolSettings : IMessageRecognizerAppSettings
    {
        private const string CATEGORY = "MessageRecognizer StartAndEndSymbol";

        [EncodingWebName]
        [Required]
        [Category(CATEGORY)]
        public string Encoding { get; set; } = "utf-8";

        [Category(CATEGORY)]
        [Required]
        [TextAndHexadecimalEdit(nameof(Encoding))]
        public string StartSymbols { get; set; } = string.Empty;

        [Category(CATEGORY)]
        [Required]
        [TextAndHexadecimalEdit(nameof(Encoding))]
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
