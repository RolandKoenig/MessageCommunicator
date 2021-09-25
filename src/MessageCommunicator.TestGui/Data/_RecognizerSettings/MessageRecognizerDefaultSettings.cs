using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MessageCommunicator.TestGui.Data
{
    [TypeAlias("MessageRecognizerDefaultSettings")]
    public class MessageRecognizerDefaultSettings : IMessageRecognizerAppSettings
    {
        private const string CATEGORY = "MessageRecognizer Default";

        [EncodingWebName]
        [Required]
        [Category(CATEGORY)]
        [Description("Character encoding for communication.")]
        public string Encoding { get; set; } = "utf-8";

        /// <inheritdoc />
        public MessageRecognizerSettings CreateLibSettings()
        {
            return new DefaultMessageRecognizerSettings(
                System.Text.Encoding.GetEncoding(this.Encoding));
        }
    }
}
