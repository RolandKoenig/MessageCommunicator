using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator
{
    public class DefaultMessageRecognizerSettings : MessageRecognizerSettings
    {
        public Encoding Encoding { get; set; }

        public DefaultMessageRecognizerSettings(Encoding encoding)
        {
            this.Encoding = encoding;
        }

        /// <inheritdoc />
        public override MessageRecognizer CreateMessageRecognizer()
        {
            return new DefaultMessageRecognizer(this.Encoding);
        }
    }
}
