using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MessageCommunicator.TestGui.Data
{
    [TypeAlias("MessageRecognizerFixedLengthSettings")]
    public class MessageRecognizerFixedLengthSettings : IMessageRecognizerAppSettings
    {
        private const string CATEGORY = "MessageRecognizer FixedLength";

        [EncodingWebName]
        [Required]
        [Category(CATEGORY)]
        [Description("Character encoding for communication.")]
        public string Encoding { get; set; } = "utf-8";

        [Category(CATEGORY)]
        [Range(1, int.MaxValue)]
        [DisplayName("Length Including EndSymbols")]
        [Description("Fixed length of the message.")]
        public int LengthIncludingEndSymbols { get; set; } = 100;

        [Category(CATEGORY)]
        [DisplayName("Fill Symbol")]
        [Description("The symbol with which to fill unused space in the message.")]
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
