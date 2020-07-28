using System;
using System.Collections.Generic;

namespace MessageCommunicator
{
    public abstract class ReconnectWaitTimeGetter
    {
        public abstract TimeSpan GetWaitTime(int errorCountSinceLastConnect);
    }
}
