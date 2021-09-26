using System;

namespace FirLib.Core.Patterns.Messaging
{
    /// <summary>
    /// This class holds all information about message subscriptions. It implements IDisposable for unsubscribing
    /// from the message.
    /// </summary>
    public class MessageSubscription : IDisposable, ICheckDisposed
    {
        // Main members for publishing
        private FirLibMessenger? _messenger;
        private Type? _messageType;
        private Delegate? _targetHandler;

        /// <summary>
        /// Is this subscription valid?
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return
                    (_messenger == null) ||
                    (_messageType == null) ||
                    (_targetHandler == null);
            }
        }

        /// <summary>
        /// Gets the corresponding Messenger object.
        /// </summary>
        public FirLibMessenger Messenger => _messenger ?? throw new ObjectDisposedException(nameof(MessageSubscription));

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        public Type MessageType => _messageType ?? throw new ObjectDisposedException(nameof(MessageSubscription));

        /// <summary>
        /// Gets the name of the message type.
        /// </summary>
        public string MessageTypeName => _messageType?.FullName ?? throw new ObjectDisposedException(nameof(MessageSubscription));

        /// <summary>
        /// Gets the name of the target object.
        /// </summary>
        public string TargetObjectName => 
            _targetHandler?.Target?.ToString() ??
            throw new ObjectDisposedException(nameof(MessageSubscription));

        /// <summary>
        /// Gets the name of the target method.
        /// </summary>
        public string TargetMethodName => _targetHandler?.Method.Name ?? throw new ObjectDisposedException(nameof(MessageSubscription));

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSubscription"/> class.
        /// </summary>
        /// <param name="messenger">The messenger.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="targetHandler">The target handler.</param>
        internal MessageSubscription(FirLibMessenger messenger, Type messageType, Delegate targetHandler)
        {
            _messenger = messenger;
            _messageType = messageType;
            _targetHandler = targetHandler;
        }

        /// <summary>
        /// Unsubscribes this subscription.
        /// </summary>
        public void Unsubscribe()
        {
            this.Dispose();
        }

        /// <summary>
        /// Sends the given message to the target.
        /// </summary>
        /// <typeparam name="TMessageType">Type of the message.</typeparam>
        /// <param name="message">The message to be published.</param>
        internal void Publish<TMessageType>(TMessageType message)
        {
            // Call this subscription
            if (!this.IsDisposed)
            {
                var targetDelegate = _targetHandler as Action<TMessageType>;
                targetDelegate!.Invoke(message);
            }
        }

        /// <summary>
        /// Clears this message.
        /// </summary>
        internal void Clear()
        {
            _messenger = null;
            _messageType = null;
            _targetHandler = null;
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                _messenger?.Unsubscribe(this);
            }
        }
    }
}
