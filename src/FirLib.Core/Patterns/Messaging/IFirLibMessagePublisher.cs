using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FirLib.Core.Patterns.Messaging;

public interface IFirLibMessagePublisher
{
    /// <summary>
    /// Sends the given message to all subscribers (async processing).
    /// There is no possibility here to wait for the answer.
    /// </summary>
    void BeginPublish<TMessageType>()
        where TMessageType : new();

    /// <summary>
    /// Sends the given message to all subscribers (async processing).
    /// There is no possibility here to wait for the answer.
    /// </summary>
    /// <typeparam name="TMessageType">The type of the message type.</typeparam>
    /// <param name="message">The message.</param>
    void BeginPublish<TMessageType>(
        TMessageType message);

    /// <summary>
    /// Sends the given message to all subscribers (async processing).
    /// The returned task waits for all synchronous subscriptions.
    /// </summary>
    /// <typeparam name="TMessageType">The type of the message.</typeparam>
    Task BeginPublishAsync<TMessageType>()
        where TMessageType : new();

    /// <summary>
    /// Sends the given message to all subscribers (async processing).
    /// The returned task waits for all synchronous subscriptions.
    /// </summary>
    /// <typeparam name="TMessageType">The type of the message.</typeparam>
    /// <param name="message">The message to be sent.</param>
    Task BeginPublishAsync<TMessageType>(
        TMessageType message);

    /// <summary>
    /// Sends the given message to all subscribers (sync processing).
    /// </summary>
    void Publish<TMessageType>()
        where TMessageType : new();

    /// <summary>
    /// Sends the given message to all subscribers (sync processing).
    /// </summary>
    /// <typeparam name="TMessageType">Type of the message.</typeparam>
    /// <param name="message">The message to send.</param>
    void Publish<TMessageType>(
        TMessageType message);
}