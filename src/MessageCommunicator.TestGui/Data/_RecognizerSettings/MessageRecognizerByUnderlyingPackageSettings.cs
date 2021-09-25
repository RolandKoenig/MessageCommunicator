using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MessageCommunicator.TestGui.Data
{
    [TypeAlias("MessageRecognizerByUnderlyingPackageSettings")]
    public class MessageRecognizerByUnderlyingPackageSettings : IMessageRecognizerAppSettings
    {
        private const string CATEGORY = "MessageRecognizer ByUnderlyingPackage";

        [EncodingWebName]
        [Required]
        [Category(CATEGORY)]
        [Description("Character encoding for communication.")]
        public string Encoding { get; set; } = "utf-8";

        /// <inheritdoc />
        public MessageRecognizerSettings CreateLibSettings()
        {
            return new ByUnderlyingPackageMessageRecognizerSettings(
                System.Text.Encoding.GetEncoding(this.Encoding));
        }
    }
}
