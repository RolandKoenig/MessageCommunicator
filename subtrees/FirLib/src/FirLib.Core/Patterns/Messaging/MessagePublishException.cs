using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FirLib.Core.Patterns.Messaging;

/// <summary>
/// An exception info holding all information about exceptions occurred during publishing a message.
/// </summary>
public class MessagePublishException : FirLibException
{
    /// <summary>
    /// Gets the type of the message.
    /// </summary>
    public Type MessageType { get; }

    /// <summary>
    /// Gets a list containing all exceptions.
    /// </summary>
    public List<Exception> PublishExceptions { get; }

    public string TrueStackTrace { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessagePublishException"/> class.
    /// </summary>
    /// <param name="messageType">Type of the message.</param>
    /// <param name="message">The message.</param>
    public MessagePublishException(Type messageType, string message)
        : base("Exceptions where raised while processing message of type " + messageType.FullName + ": " + message)
    {
        this.MessageType = messageType;
        this.PublishExceptions = new List<Exception>();

        // Acquire true stacktrace information
        this.TrueStackTrace = (new StackTrace()).ToString();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessagePublishException"/> class.
    /// </summary>
    /// <param name="messageType">Type of the message.</param>
    /// <param name="publishExceptions">Exceptions raised during publish process.</param>
    public MessagePublishException(Type messageType, List<Exception> publishExceptions)
        : base("Exceptions where raised while processing message of type " + messageType.FullName + "!")
    {
        this.MessageType = messageType;
        this.PublishExceptions = publishExceptions;

        // Acquire true stacktrace information
        this.TrueStackTrace = (new StackTrace()).ToString();
    }
}