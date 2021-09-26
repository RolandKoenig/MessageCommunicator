using System;
using System.Collections.Generic;
using System.Text;

namespace FirLib.Core.Patterns.Messaging
{
    public interface IFirLibMessageSubscriber
    {
        /// <summary>
        /// Subscribes all receiver-methods of the given target object to this Messenger.
        /// The messages have to be called OnMessageReceived.
        /// </summary>
        /// <param name="targetObject">The target object which is to subscribe.</param>
        IEnumerable<MessageSubscription> SubscribeAll(object targetObject);

        /// <summary>
        /// Subscribes to the given MessageType.
        /// </summary>
        /// <typeparam name="TMessageType">Type of the message.</typeparam>
        /// <param name="actionOnMessage">Action to perform on incoming message.</param>
        MessageSubscription Subscribe<TMessageType>(Action<TMessageType> actionOnMessage);

        /// <summary>
        /// Subscribes to the given message type.
        /// </summary>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="actionOnMessage">Action to perform on incoming message.</param>
        MessageSubscription Subscribe(
            Delegate actionOnMessage, Type messageType);
    }
}
