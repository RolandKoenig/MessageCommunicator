using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MessageCommunicator.TestGui.Data
{
    public class MessageRecognizerFixedLengthAndEndSymbolsSettings : IMessageRecognizerAppSettings
    {
        private const string CATEGORY = "FixedLength and EndSymbols Recognizer";

        [EncodingWebName]
        [Category(CATEGORY)]
        [Required]
        public string Encoding { get; set; } = "utf-8";

        [Category(CATEGORY)]
        [Required]
        [TextAndHexadecimalEdit(nameof(Encoding))]
        public string EndSymbols { get; set; } = string.Empty;

        [Category(CATEGORY)]
        [Range(1, int.MaxValue)]
        public int LengthIncludingEndSymbols { get; set; } = 100;

        [Category(CATEGORY)]
        public char FillSymbol { get; set; } = '.';

        /// <inheritdoc />
        public MessageRecognizerSettings CreateLibSettings()
        {
            return new FixedLengthAndEndSymbolsMessageRecognizerSettings(
                System.Text.Encoding.GetEncoding(this.Encoding),
                this.EndSymbols,
                this.LengthIncludingEndSymbols,
                this.FillSymbol);
        }
    }
}
