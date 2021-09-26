using System;
using System.Reflection;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FirLib.Core.Checking;
using FirLib.Core.Patterns.BackgroundLoops;

namespace FirLib.Core.Patterns.Messaging
{
    /// <summary>
    /// Main class for sending/receiving messages.
    /// This class follows the Messenger pattern but modifies it on some parts like thread synchronization.
    /// What 'messenger' actually is, see here a short explanation:
    /// http://stackoverflow.com/questions/22747954/mvvm-light-toolkit-messenger-uses-event-aggregator-or-mediator-pattern
    /// </summary>
    public class FirLibMessenger : IFirLibMessagePublisher, IFirLibMessageSubscriber
    {
        public const string METHOD_NAME_MESSAGE_RECEIVED = "OnMessageReceived";

        /// <summary>
        /// Gets or sets a custom exception handler which is used globally.
        /// Return true to skip default exception behavior (exception is thrown to publisher by default).
        /// </summary>
        public static Func<FirLibMessenger, Exception, bool>? CustomPublishExceptionHandler;

        // Global synchronization (enables the possibility to publish a message over more messengers / areas of the application)
        private static ConcurrentDictionary<string, FirLibMessenger> s_messengersByName;

        // Global information about message routing
        private static ConcurrentDictionary<Type, string[]> s_messagesAsyncTargets;
        private static ConcurrentDictionary<Type, string[]> s_messageSources;

        // Checking and global synchronization
        private string _globalMessengerName;
        private SynchronizationContext? _hostSyncContext;
        private FirLibMessengerThreadingBehavior _publishCheckBehavior;

        // Message subscriptions
        private Dictionary<Type, List<MessageSubscription>> _messageSubscriptions;
        private object _messageSubscriptionsLock;

        /// <summary>
        /// Gets or sets the synchronization context on which to publish all messages.
        /// </summary>
        public SynchronizationContext? HostSyncContext
        {
            get { return _hostSyncContext; }
        }

        /// <summary>
        /// Gets the current threading behavior of this Messenger.
        /// </summary>
        public FirLibMessengerThreadingBehavior ThreadingBehavior
        {
            get { return _publishCheckBehavior; }
        }

        /// <summary>
        /// Gets the name of the associated thread.
        /// </summary>
        public string MainThreadName
        {
            get
            {
                return _globalMessengerName;
            }
        }

        /// <summary>
        /// Counts all message subscriptions.
        /// </summary>
        public int CountSubscriptions
        {
            get
            {
                lock (_messageSubscriptionsLock)
                {
                    var totalCount = 0;
                    foreach (var actKeyValuePair in _messageSubscriptions)
                    {
                        totalCount += actKeyValuePair.Value.Count;
                    }
                    return totalCount;
                }
            }
        }

        public Func<SynchronizationContext, SynchronizationContext, bool>? CustomSynchronizationContextEqualityChecker { get; set; }

        /// <summary>
        /// Gets the total count of globally registered messengers.
        /// </summary>
        public static int CountGlobalMessengers
        {
            get { return s_messengersByName.Count; }
        }

        /// <summary>
        /// Initializes the <see cref="FirLib.Core.Patterns.Messaging.FirLibMessenger" /> class.
        /// </summary>
        static FirLibMessenger()
        {
            s_messengersByName = new ConcurrentDictionary<string, FirLibMessenger>();
            s_messagesAsyncTargets = new ConcurrentDictionary<Type, string[]>();
            s_messageSources = new ConcurrentDictionary<Type, string[]>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirLib.Core.Patterns.Messaging.FirLibMessenger"/> class.
        /// </summary>
        public FirLibMessenger()
        {
            _globalMessengerName = string.Empty;
            _hostSyncContext = null;
            _publishCheckBehavior = FirLibMessengerThreadingBehavior.Ignore;

            _messageSubscriptions = new Dictionary<Type, List<MessageSubscription>>();
            _messageSubscriptionsLock = new object();
        }

        /// <summary>
        /// Gets the <see cref="FirLib.Core.Patterns.Messaging.FirLibMessenger"/> by the given name.
        /// </summary>
        /// <param name="messengerName">The name of the messenger.</param>
        public static FirLibMessenger GetByName(string messengerName)
        {
            messengerName.EnsureNotNullOrEmpty(nameof(messengerName));

            var result = TryGetByName(messengerName);
            if (result == null) { throw new FirLibException($"Unable to find Messenger for thread {messengerName}!"); }
            return result;
        }

        /// <summary>
        /// Gets the <see cref="FirLib.Core.Patterns.Messaging.FirLibMessenger"/> by the given name.
        /// </summary>
        /// <param name="messengerName">The name of the messenger.</param>
        public static FirLibMessenger? TryGetByName(string messengerName)
        {
            messengerName.EnsureNotNullOrEmpty(nameof(messengerName));

            s_messengersByName.TryGetValue(messengerName, out var result);
            return result;
        }

        /// <summary>
        /// Sets all required properties based on the given target thread.
        /// The name of the messenger is directly taken from the given <see cref="BackgroundLoop"/>.
        /// </summary>
        /// <param name="hostThread">The thread on which this Messenger should work on.</param>
        public void ConnectToGlobalMessaging(BackgroundLoop hostThread)
        {
            hostThread.EnsureNotNull(nameof(hostThread));

            this.ConnectToGlobalMessaging(
                FirLibMessengerThreadingBehavior.EnsureMainSyncContextOnSyncCalls,
                hostThread.Name,
                hostThread.SyncContext);
        }

        /// <summary>
        /// Sets all required synchronization properties.
        /// </summary>
        /// <param name="checkBehavior">Defines the checking behavior for publish calls.</param>
        /// <param name="messengerName">The name by which this messenger should be registered.</param>
        /// <param name="hostSyncContext">The synchronization context to be used.</param>
        public void ConnectToGlobalMessaging(FirLibMessengerThreadingBehavior checkBehavior, string messengerName, SynchronizationContext? hostSyncContext)
        {
            messengerName.EnsureNotNullOrEmpty(nameof(messengerName));

            if (!string.IsNullOrEmpty(_globalMessengerName))
            {
                throw new FirLibException($"This messenger is already registered as '{_globalMessengerName}'!");
            }
            if (s_messengersByName.ContainsKey(messengerName))
            {
                throw new FirLibException($"The name '{messengerName}' is already in use by another messenger!");
            }

            _globalMessengerName = messengerName;
            _publishCheckBehavior = checkBehavior;
            _hostSyncContext = hostSyncContext;

            if (!string.IsNullOrEmpty(messengerName))
            {
                s_messengersByName.TryAdd(messengerName, this);
            }
        }

        /// <summary>
        /// Clears all synchronization configuration.
        /// </summary>
        public void DisconnectFromGlobalMessaging()
        {
            if (string.IsNullOrEmpty(_globalMessengerName)) { return; }

            s_messengersByName.TryRemove(_globalMessengerName, out _);

            _publishCheckBehavior = FirLibMessengerThreadingBehavior.Ignore;
            _globalMessengerName = string.Empty;
            _hostSyncContext = null;
        }

        /// <summary>
        /// Gets a collection containing all active subscriptions.
        /// </summary>
        public List<MessageSubscription> GetAllSubscriptions()
        {
            List<MessageSubscription> result = new();

            lock (_messageSubscriptionsLock)
            {
                foreach (var actPair in _messageSubscriptions)
                {
                    foreach(var actSubscription in actPair.Value)
                    {
                        result.Add(actSubscription);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Waits for the given message.
        /// </summary>
        public Task<T> WaitForMessageAsync<T>()
        {
            TaskCompletionSource<T> taskComplSource = new(TaskCreationOptions.RunContinuationsAsynchronously);

            MessageSubscription? subscription = null;
            subscription = this.Subscribe<T>((message) =>
            {
                // Unsubscribe first
                subscription!.Unsubscribe();

                // Set the task's result
                taskComplSource.SetResult(message);
            });

            return taskComplSource.Task;
        }

        /// <summary>
        /// Subscribes all receiver-methods of the given target object to this Messenger.
        /// The messages have to be called OnMessageReceived.
        /// </summary>
        /// <param name="targetObject">The target object which is to subscribe.</param>
        public IEnumerable<MessageSubscription> SubscribeAll(object targetObject)
        {
            targetObject.EnsureNotNull(nameof(targetObject));

            Type targetObjectType = targetObject.GetType();
            List<MessageSubscription> generatedSubscriptions = new(16);
            try
            {
                foreach (MethodInfo actMethod in targetObjectType.GetMethods(
                    BindingFlags.NonPublic | BindingFlags.Public | 
                    BindingFlags.Instance | BindingFlags.InvokeMethod))
                {
                    if (!actMethod.Name.Equals(METHOD_NAME_MESSAGE_RECEIVED)) { continue; }

                    ParameterInfo[] parameters = actMethod.GetParameters();
                    if (parameters.Length != 1) { continue; }

                    if (!FirLibMessageHelper.ValidateMessageType(parameters[0].ParameterType, out _))
                    {
                        continue;
                    }

                    Type genericAction = typeof(Action<>);
                    Type delegateType = genericAction.MakeGenericType(parameters[0].ParameterType);
                    generatedSubscriptions.Add(this.Subscribe(
                        actMethod.CreateDelegate(delegateType, targetObject),
                        parameters[0].ParameterType));
                }
            }
            catch(Exception)
            {
                foreach(MessageSubscription actSubscription in generatedSubscriptions)
                {
                    actSubscription.Unsubscribe();
                }
                generatedSubscriptions.Clear();
            }

            return generatedSubscriptions;
        }

        /// <summary>
        /// Subscribes to the given MessageType.
        /// </summary>
        /// <typeparam name="TMessageType">Type of the message.</typeparam>
        /// <param name="actionOnMessage">Action to perform on incoming message.</param>
        public MessageSubscription Subscribe<TMessageType>(Action<TMessageType> actionOnMessage)
        {
            actionOnMessage.EnsureNotNull(nameof(actionOnMessage));

            Type currentType = typeof(TMessageType);
            return this.Subscribe(actionOnMessage, currentType);
        }

        /// <summary>
        /// Subscribes to the given message type.
        /// </summary>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="actionOnMessage">Action to perform on incoming message.</param>
        public MessageSubscription Subscribe(
            Delegate actionOnMessage, Type messageType)
        {
            actionOnMessage.EnsureNotNull(nameof(actionOnMessage));
            messageType.EnsureNotNull(nameof(messageType));

            FirLibMessageHelper.EnsureValidMessageType(messageType);

            MessageSubscription newOne = new(this, messageType, actionOnMessage);
            lock (_messageSubscriptionsLock)
            {
                if (_messageSubscriptions.ContainsKey(messageType))
                {
                    _messageSubscriptions[messageType].Add(newOne);
                }
                else
                {
                    List<MessageSubscription> newList = new();
                    newList.Add(newOne);
                    _messageSubscriptions[messageType] = newList;
                }
            }

            return newOne;
        }

        /// <summary>
        /// Clears the given MessageSubscription.
        /// </summary>
        /// <param name="messageSubscription">The subscription to clear.</param>
        public void Unsubscribe(MessageSubscription messageSubscription)
        {
            messageSubscription.EnsureNotNull(nameof(messageSubscription));

            if (!messageSubscription.IsDisposed)
            {
                Type messageType = messageSubscription.MessageType;

                // Remove subscription from internal list

                lock (_messageSubscriptionsLock)
                {
                    if (_messageSubscriptions.ContainsKey(messageType))
                    {
                        List<MessageSubscription> listOfSubscriptions = _messageSubscriptions[messageType];
                        listOfSubscriptions.Remove(messageSubscription);
                        if (listOfSubscriptions.Count == 0)
                        {
                            _messageSubscriptions.Remove(messageType);
                        }
                    }
                }

                // Clear given subscription
                messageSubscription.Clear();
            }
        }

        /// <summary>
        /// Counts all message subscriptions for the given message type.
        /// </summary>
        /// <typeparam name="TMessageType">The type of the message for which to count all subscriptions.</typeparam>
        public int CountSubscriptionsForMessage<TMessageType>()
        {
            lock (_messageSubscriptionsLock)
            {
                if (_messageSubscriptions.TryGetValue(typeof(TMessageType), out var subscriptions))
                {
                    return subscriptions.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Sends the given message to all subscribers (async processing).
        /// There is no possibility here to wait for the answer.
        /// </summary>
        public void BeginPublish<TMessageType>()
            where TMessageType : new()
        {
            this.BeginPublish(new TMessageType());
        }

        /// <summary>
        /// Sends the given message to all subscribers (async processing).
        /// There is no possibility here to wait for the answer.
        /// </summary>
        /// <typeparam name="TMessageType">The type of the message type.</typeparam>
        /// <param name="message">The message.</param>
        public void BeginPublish<TMessageType>(
            TMessageType message)
        {
            _hostSyncContext.PostAlsoIfNull(() => this.Publish(message));
        }

        /// <summary>
        /// Sends the given message to all subscribers (async processing).
        /// The returned task waits for all synchronous subscriptions.
        /// </summary>
        /// <typeparam name="TMessageType">The type of the message.</typeparam>
        public Task BeginPublishAsync<TMessageType>()
            where TMessageType : new()
        {
            return _hostSyncContext.PostAlsoIfNullAsync(
                this.Publish<TMessageType>);
        }

        /// <summary>
        /// Sends the given message to all subscribers (async processing).
        /// The returned task waits for all synchronous subscriptions.
        /// </summary>
        /// <typeparam name="TMessageType">The type of the message.</typeparam>
        /// <param name="message">The message to be sent.</param>
        public Task BeginPublishAsync<TMessageType>(
            TMessageType message)
        {
            return _hostSyncContext.PostAlsoIfNullAsync(
                () => this.Publish(message));
        }

        /// <summary>
        /// Sends the given message to all subscribers (sync processing).
        /// </summary>
        public void Publish<TMessageType>()
            where TMessageType : new()
        {
            this.Publish(new TMessageType());
        }

        /// <summary>
        /// Sends the given message to all subscribers (sync processing).
        /// </summary>
        /// <typeparam name="TMessageType">Type of the message.</typeparam>
        /// <param name="message">The message to send.</param>
        public void Publish<TMessageType>(
            TMessageType message)
        {
            this.PublishInternal(
                message, true);
        }

        /// <summary>
        /// Sends the given message to all subscribers (sync processing).
        /// </summary>
        /// <typeparam name="TMessageType">Type of the message.</typeparam>
        /// <param name="message">The message to send.</param>
        /// <param name="isInitialCall">Is this one the initial call to publish? (false if we are coming from async routing)</param>
        private void PublishInternal<TMessageType>(
            TMessageType message, bool isInitialCall)
        {
            FirLibMessageHelper.EnsureValidMessageTypeAndValue(message);

            try
            {
                // Check whether publish is possible
                if(_publishCheckBehavior == FirLibMessengerThreadingBehavior.EnsureMainSyncContextOnSyncCalls)
                {
                    if (!this.CompareSynchronizationContexts())
                    {
                        throw new FirLibException(
                            "Unable to perform a synchronous publish call because current " +
                            "SynchronizationContext is set wrong. This indicates that the call " +
                            "comes from a wrong thread!");
                    }
                }

                // Check for correct message sources
                Type currentType = typeof(TMessageType);
                if (isInitialCall)
                {
                    string[] possibleSources = s_messageSources.GetOrAdd(currentType, (_) => FirLibMessageHelper.GetPossibleSourceMessengersOfMessageType(currentType));
                    if (possibleSources.Length > 0)
                    {
                        string mainThreadName = _globalMessengerName;
                        if (string.IsNullOrEmpty(mainThreadName) ||
                            (Array.IndexOf(possibleSources, mainThreadName) < 0))
                        {
                            throw new InvalidOperationException(
                                $"The message of type {currentType.FullName} can only be sent " +
                                $"by the threads [{possibleSources.ToCommaSeparatedString()}]. This Messenger " +
                                $"belongs to the thread {(!string.IsNullOrEmpty(mainThreadName) ? mainThreadName : "(empty)")}, " +
                                "so no publish possible!");
                        }
                    }
                }

                // Perform synchronous message handling
                List<MessageSubscription> subscriptionsToTrigger = new(20);
                lock (_messageSubscriptionsLock)
                {
                    if (_messageSubscriptions.ContainsKey(currentType))
                    {
                        // Need to copy the list to avoid problems, when the list is changed during the loop and cross thread accesses
                        subscriptionsToTrigger = new List<MessageSubscription>(_messageSubscriptions[currentType]);
                    }
                }

                // Trigger all found subscriptions
                List<Exception>? occurredExceptions = null;
                for (var loop = 0; loop < subscriptionsToTrigger.Count; loop++)
                {
                    try
                    {
                        subscriptionsToTrigger[loop].Publish(message);
                    }
                    catch (Exception ex)
                    {
                        occurredExceptions ??= new List<Exception>();
                        occurredExceptions.Add(ex);
                    }
                }

                // Perform further message routing if enabled
                if (isInitialCall)
                {
                    // Get information about message routing
                    string[] asyncTargets = s_messagesAsyncTargets.GetOrAdd(currentType, (_) => FirLibMessageHelper.GetAsyncRoutingTargetMessengersOfMessageType(currentType));
                    string mainThreadName = _globalMessengerName;
                    for (var loop = 0; loop < asyncTargets.Length; loop++)
                    {
                        string actAsyncTargetName = asyncTargets[loop];
                        if (mainThreadName == actAsyncTargetName) { continue; }

                        if (s_messengersByName.TryGetValue(actAsyncTargetName, out var actAsyncTargetHandler))
                        {
                            var actSyncContext = actAsyncTargetHandler!._hostSyncContext;
                            if (actSyncContext == null) { continue; }

                            FirLibMessenger innerHandlerForAsyncCall = actAsyncTargetHandler!;
                            actSyncContext.PostAlsoIfNull(() =>
                            {
                                innerHandlerForAsyncCall.PublishInternal(message, false);
                            });
                        }
                    }
                }

                // Notify all exceptions occurred during publish progress
                if (isInitialCall)
                {
                    if ((occurredExceptions != null) &&
                        (occurredExceptions.Count > 0))
                    {
                        throw new MessagePublishException(typeof(TMessageType), occurredExceptions);
                    }
                }
            }
            catch (Exception ex)
            {
                // Check whether we have to throw the exception globally
                var globalExceptionHandler = CustomPublishExceptionHandler;
                var doRaise = true;
                if (globalExceptionHandler != null)
                {
                    try
                    {
                        doRaise = !globalExceptionHandler(this, ex);
                    }
                    catch 
                    {
                        doRaise = true;
                    }
                }

                // Raise the exception to inform caller about it
                if (doRaise) { throw; }
            }
        }

        /// <summary>
        /// Compares the SynchronizationContext of the current thread and of this messenger instance.
        /// </summary>
        private bool CompareSynchronizationContexts()
        {
            var currentSynchronizationContext = SynchronizationContext.Current;
            if((currentSynchronizationContext != null) && (_hostSyncContext != null))
            {
                var syncContextEqualityChecker = this.CustomSynchronizationContextEqualityChecker;
                if (syncContextEqualityChecker != null)
                {
                    return syncContextEqualityChecker(currentSynchronizationContext, _hostSyncContext);
                }
                else
                {
                    return SynchronizationContext.Current == _hostSyncContext;
                }
            }
            else if((currentSynchronizationContext != null) || (_hostSyncContext != null))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
