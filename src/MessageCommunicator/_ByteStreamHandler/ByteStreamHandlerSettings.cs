using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator
{
    /// <summary>
    /// A container for all settings regarding a <see cref="ByteStreamHandler"/> object.
    /// </summary>
    public abstract class ByteStreamHandlerSettings
    {
        /// <summary>
        /// The factory method for a <see cref="ByteStreamHandler"/> implementation.
        /// </summary>
        /// <returns>A new <see cref="ByteStreamHandler"/> with configuration from this instance.</returns>
        public abstract ByteStreamHandler CreateByteStreamHandler();
    }
}
