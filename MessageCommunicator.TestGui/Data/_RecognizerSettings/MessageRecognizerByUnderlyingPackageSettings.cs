using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;

namespace MessageCommunicator.TestGui.Data
{
    public class MessageRecognizerByUnderlyingPackageSettings : IMessageRecognizerAppSettings
    {
        private const string CATEGORY = "ByUnderlyingPackage Recognizer";

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
