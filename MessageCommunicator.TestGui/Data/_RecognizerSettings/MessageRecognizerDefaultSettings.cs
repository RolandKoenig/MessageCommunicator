using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MessageCommunicator.TestGui.Data
{
    [TypeAlias("MessageRecognizerDefaultSettings")]
    public class MessageRecognizerDefaultSettings : IMessageRecognizerAppSettings
    {
        private const string CATEGORY = "Default Recognizer";

        [EncodingWebName]
        [Category(CATEGORY)]
        public string Encoding { get; set; } = "utf-8";

        /// <inheritdoc />
        public MessageRecognizerSettings CreateLibSettings()
        {
            return new DefaultMessageRecognizerSettings(
                System.Text.Encoding.GetEncoding(this.Encoding));
        }
    }
}
