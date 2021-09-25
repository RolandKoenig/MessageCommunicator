using System;
using System.Collections.Generic;
using System.Text;
using Light.GuardClauses;

namespace MessageCommunicator
{
    /// <summary>
    /// This class provides all settings for <see cref="FixedLengthMessageRecognizer"/>.
    /// </summary>
    public class FixedLengthMessageRecognizerSettings : MessageRecognizerSettings
    {
        /// <summary>
        /// Gets or sets the <see cref="Encoding"/> to be used when convert characters to/from bytes.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Gets or sets the total length of received/sent messages.
        /// </summary>
        public int Length { get; set; }
         
        /// <summary>
        /// Gets or sets the fill symbol for messages shorter than the fixed length.
        /// </summary>
        public char FillSymbol { get; set; }

        /// <summary>
        /// Creates a new <see cref="FixedLengthMessageRecognizerSettings"/> instance.
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> to be used when convert characters to/from bytes.</param>
        /// <param name="length">Total length of received/sent messages.</param>
        /// <param name="fillSymbol">Fill symbol for messages shorter than the fixed length.</param>
        public FixedLengthMessageRecognizerSettings(Encoding encoding, int length, char fillSymbol)
        {
            encoding.MustNotBeNull(nameof(encoding));
            length.MustBeGreaterThan(0, nameof(length));

            this.Encoding = encoding;
            this.Length = length;
            this.FillSymbol = fillSymbol;
        }

        /// <inheritdoc />
        public override MessageRecognizer CreateMessageRecognizer()
        {
            return new FixedLengthMessageRecognizer(this.Encoding, this.Length, this.FillSymbol);
        }
    }
}
