using System;
using System.Collections.Generic;
using System.Text;
using Light.GuardClauses;

namespace MessageCommunicator
{
    /// <summary>
    /// This class provides all settings for <see cref="DefaultMessageRecognizer"/>.
    /// </summary>
    public class DefaultMessageRecognizerSettings : MessageRecognizerSettings
    {
        /// <summary>
        /// Gets or sets the <see cref="Encoding"/> to be used when convert characters to/from bytes.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Creates a new <see cref="DefaultMessageRecognizerSettings"/> instance.
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> to be used when convert characters to/from bytes.</param>
        public DefaultMessageRecognizerSettings(Encoding encoding)
        {
            encoding.MustNotBeNull(nameof(encoding));

            this.Encoding = encoding;
        }

        /// <inheritdoc />
        public override MessageRecognizer CreateMessageRecognizer()
        {
            return new DefaultMessageRecognizer(this.Encoding);
        }
    }
}
