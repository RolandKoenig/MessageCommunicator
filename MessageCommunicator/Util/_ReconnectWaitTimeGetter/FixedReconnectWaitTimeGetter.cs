using System;
using System.Collections.Generic;

namespace MessageCommunicator
{
    public class FixedReconnectWaitTimeGetter : ReconnectWaitTimeGetter
    {
        private readonly TimeSpan _fixedWaitTime;

        public FixedReconnectWaitTimeGetter(TimeSpan fixedWaitTime)
        {
            _fixedWaitTime = fixedWaitTime;
        }

        public override TimeSpan GetWaitTime(int errorCountSinceLastConnect)
        {
            return _fixedWaitTime;
        }
    }
}
