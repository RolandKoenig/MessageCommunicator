using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MessageCommunicator.TestGui.Data
{
    [TypeAlias("MessageRecognizerByUnderlyingPackageSettings")]
    public class MessageRecognizerByUnderlyingPackageSettings : IMessageRecognizerAppSettings
    {
        private const string CATEGORY = "MessageRecognizer ByUnderlyingPackage";

        [EncodingWebName]
        [Category(CATEGORY)]
        public string Encoding { get; set; } = "utf-8";

        /// <inheritdoc />
        public MessageRecognizerSettings CreateLibSettings()
        {
            return new ByUnderlyingPackageMessageRecognizerSettings(
                System.Text.Encoding.GetEncoding(this.Encoding));
        }
    }
}
