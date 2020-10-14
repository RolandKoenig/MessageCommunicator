using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator.Util
{
    public interface IMessageChannelConnectionObserver
    {
        void RegisterMessageChannel(MessageChannel channel);

        void DeregisterMessageChannel(MessageChannel channel);
    }
}
