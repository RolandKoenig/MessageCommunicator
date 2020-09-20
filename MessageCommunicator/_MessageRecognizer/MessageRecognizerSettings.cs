using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator
{
    /// <summary>
    /// Encapsulates all settings for an <see cref="IMessageRecognizer"/> implementation.
    /// </summary>
    public abstract class MessageRecognizerSettings
    {
        /// <summary>
        /// Factory method which creates an <see cref="IMessageRecognizer"/> implementation.
        /// </summary>
        /// <returns>The created <see cref="IMessageRecognizer"/> implementation.</returns>
        public abstract MessageRecognizer CreateMessageRecognizer();
    }
}
