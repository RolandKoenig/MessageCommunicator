using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MessageCommunicator.TestGui.Data
{
    [TypeAlias("MessageRecognizerFixedLengthSettings")]
    public class MessageRecognizerFixedLengthSettings : IMessageRecognizerAppSettings
    {
        private const string CATEGORY = "MessageRecognizer FixedLength";

        [EncodingWebName]
        [Category(CATEGORY)]
        [Required]
        public string Encoding { get; set; } = "utf-8";

        [Category(CATEGORY)]
        [Range(1, int.MaxValue)]
        public int LengthIncludingEndSymbols { get; set; } = 100;

        [Category(CATEGORY)]
        public char FillSymbol { get; set; } = '.';

        /// <inheritdoc />
        public MessageRecognizerSettings CreateLibSettings()
        {
            return new FixedLengthMessageRecognizerSettings(
                System.Text.Encoding.GetEncoding(this.Encoding),
                this.LengthIncludingEndSymbols,
                this.FillSymbol);
        }
    }
}
