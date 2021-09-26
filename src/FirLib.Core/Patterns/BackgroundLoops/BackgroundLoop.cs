using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FirLib.Core.Checking;

namespace FirLib.Core.Patterns.BackgroundLoops
{
    public class BackgroundLoop
    {
        private const int STANDARD_HEARTBEAT = 500;

        [field: ThreadStatic]
        public static BackgroundLoop? CurrentBackgroundLoop
        {
            get;
            private set;
        }

        // Members for thread runtime
        private volatile BackgroundLoopState _currentState;
        private Thread? _mainThread;
        private CultureInfo _culture;
        private CultureInfo _uiCulture;

        // Threading resources
        private BackgroundLoopSynchronizationContext _syncContext;
        private ConcurrentQueue<Action> _taskQueue;
        private SemaphoreSlim _mainLoopSynchronizeObject;
        private SemaphoreSlim? _threadStopSynchronizeObject;

        /// <summary>
        /// Gets the current SynchronizationContext object.
        /// </summary>
        public SynchronizationContext SyncContext => _syncContext;

        /// <summary>
        /// Gets the name of this thread.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the thread's heartbeat.
        /// </summary>
        protected int HeartBeat { get; set; }

        /// <summary>
        /// Called when the thread ist starting.
        /// </summary>
        public event EventHandler? Starting;

        /// <summary>
        /// Called when the thread is stopping.
        /// </summary>
        public event EventHandler? Stopping;

        /// <summary>
        /// Called when an unhandled exception occurred.
        /// </summary>
        public event EventHandler<BackgroundLoopExceptionEventArgs>? ThreadException;

        /// <summary>
        /// Called on each heartbeat.
        /// </summary>
        public event EventHandler<BackgroundLoopTickEventArgs>? Tick;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundLoop"/> class.
        /// </summary>
        public BackgroundLoop()
            : this(string.Empty, STANDARD_HEARTBEAT)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundLoop"/> class.
        /// </summary>
        /// <param name="name">The name of the generated thread.</param>
        /// <param name="heartBeat">The initial heartbeat of the BackgroundLoop.</param>
        public BackgroundLoop(string name, int heartBeat)
        {
            _taskQueue = new ConcurrentQueue<Action>();
            _mainLoopSynchronizeObject = new SemaphoreSlim(1);

            this.Name = name;
            this.HeartBeat = heartBeat;

            _syncContext = new BackgroundLoopSynchronizationContext(this);

            _culture = Thread.CurrentThread.CurrentCulture;
            _uiCulture = Thread.CurrentThread.CurrentUICulture;
        }

        /// <summary>
        /// Starts the thread.
        /// </summary>
        public void Start()
        {
            if (_currentState != BackgroundLoopState.None) { throw new InvalidOperationException("Unable to start thread: Illegal state: " + _currentState + "!"); }

            //Ensure that one single pass of the main loop is made at once
            _mainLoopSynchronizeObject.Release();

            // Create stop semaphore
            if (_threadStopSynchronizeObject != null)
            {
                _threadStopSynchronizeObject.Dispose();
                _threadStopSynchronizeObject = null;
            }

            _threadStopSynchronizeObject = new SemaphoreSlim(0);

            //Go into starting state
            _currentState = BackgroundLoopState.Starting;

            _mainThread = new Thread(this.BackgroundLoopMainMethod)
            {
                IsBackground = true,
                Name = this.Name
            };

            _mainThread.Start();
        }

        /// <summary>
        /// Waits until this BackgroundLoop has stopped.
        /// </summary>
        public Task WaitUntilSoppedAsync()
        {
            switch (_currentState)
            {
                case BackgroundLoopState.None:
                case BackgroundLoopState.Stopping:
                    return Task.Delay(100);

                case BackgroundLoopState.Running:
                case BackgroundLoopState.Starting:
                    var taskSource = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
                    this.Stopping += (_, _) =>
                    {
                        taskSource.TrySetResult(null);
                    };
                    return taskSource.Task;

                default:
                    throw new FirLibException($"Unhandled {nameof(BackgroundLoopState)} {_currentState}!");
            }
        }

        /// <summary>
        /// Starts this thread. The returned task is completed when starting is finished.
        /// </summary>
        public Task StartAsync()
        {
            this.Start();

            return this.InvokeAsync(() => { });
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            if (_currentState != BackgroundLoopState.Running) { throw new InvalidOperationException($"Unable to stop thread: Illegal state: {_currentState}!"); }
            _currentState = BackgroundLoopState.Stopping;

            while (_taskQueue.TryDequeue(out _))
            {

            }

            //Trigger next update
            this.Trigger();
        }

        /// <summary>
        /// Stops the asynchronous.
        /// </summary>
        public async Task StopAsync(int timeout)
        {
            this.Stop();

            if (_threadStopSynchronizeObject != null)
            {
                await _threadStopSynchronizeObject.WaitAsync(timeout);

                _threadStopSynchronizeObject.Dispose();
                _threadStopSynchronizeObject = null;
            }
        }

        /// <summary>
        /// Triggers a new heartbeat.
        /// </summary>
        public virtual void Trigger()
        {
            var synchronizationObject = _mainLoopSynchronizeObject;
            synchronizationObject.Release();
        }

        /// <summary>
        /// Invokes the given delegate within the thread of this object.
        /// </summary>
        /// <param name="actionToInvoke">The delegate to invoke.</param>
        public Task InvokeAsync(Action actionToInvoke)
        {
            actionToInvoke.EnsureNotNull(nameof(actionToInvoke));

            // Enqueue the given action
            var taskCompletionSource = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);

            _taskQueue.Enqueue(() =>
            {
                try
                {
                    actionToInvoke();
                    taskCompletionSource.SetResult(null);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });

            Task result = taskCompletionSource.Task;

            //Triggers the main loop
            this.Trigger();

            //Returns the result
            return result;
        }

        /// <summary>
        /// Thread is starting.
        /// </summary>
        protected virtual void OnStarting(EventArgs eArgs)
        {
            this.Starting?.Invoke(this, eArgs);
        }

        /// <summary>
        /// Called on each tick.
        /// </summary>
        protected virtual void OnTick(BackgroundLoopTickEventArgs eArgs)
        {
            this.Tick?.Invoke(this, eArgs);
        }

        /// <summary>
        /// Called on each occurred exception.
        /// </summary>
        protected virtual void OnThreadException(BackgroundLoopExceptionEventArgs eArgs)
        {
            this.ThreadException?.Invoke(this, eArgs);
        }

        /// <summary>
        /// Thread is stopping.
        /// </summary>
        protected virtual void OnStopping(EventArgs eArgs)
        {
            this.Stopping?.Invoke(this, eArgs);
        }

        /// <summary>
        /// The thread's main method.
        /// </summary>
        private void BackgroundLoopMainMethod()
        {
            if (_mainThread == null) { return; }
            if (_mainThread != Thread.CurrentThread) { return; }

            try
            {
                _mainThread.CurrentCulture = _culture;
                _mainThread.CurrentUICulture = _uiCulture;

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                //Set synchronization context for this thread
                SynchronizationContext.SetSynchronizationContext(_syncContext);

                //Notify start process
                try
                {
                    CurrentBackgroundLoop = this;
                    this.OnStarting(EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    this.OnThreadException(new BackgroundLoopExceptionEventArgs(_currentState, ex));
                    _currentState = BackgroundLoopState.None;
                    CurrentBackgroundLoop = null;
                    return;
                }

                //Run main-thread
                if (_currentState != BackgroundLoopState.None)
                {
                    _currentState = BackgroundLoopState.Running;
                    while (_currentState == BackgroundLoopState.Running)
                    {
                        try
                        {
                            //Wait for next action to perform
                            _mainLoopSynchronizeObject.Wait(this.HeartBeat);

                            //Measure current time
                            stopWatch.Stop();
                            var elapsedTicks = stopWatch.Elapsed.Ticks;
                            stopWatch.Reset();
                            stopWatch.Start();

                            // Get current task queue
                            var localTaskQueue = new List<Action>();
                            while (_taskQueue.TryDequeue(out var dummyAction))
                            {
                                localTaskQueue.Add(dummyAction);
                            }

                            // Execute all tasks
                            foreach (var actTask in localTaskQueue)
                            {
                                try
                                {
                                    actTask();
                                }
                                catch (Exception ex)
                                {
                                    this.OnThreadException(new BackgroundLoopExceptionEventArgs(_currentState, ex));
                                }
                            }

                            // Performs a tick
                            this.OnTick(new BackgroundLoopTickEventArgs(elapsedTicks));
                        }
                        catch (Exception ex)
                        {
                            this.OnThreadException(new BackgroundLoopExceptionEventArgs(_currentState, ex));
                        }
                    }

                    // Notify stop process
                    try
                    {
                        this.OnStopping(EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        this.OnThreadException(new BackgroundLoopExceptionEventArgs(_currentState, ex));
                    }
                    CurrentBackgroundLoop = null;
                }

                // Reset state to none
                _currentState = BackgroundLoopState.None;

                stopWatch.Stop();
                stopWatch = null;
            }
            catch (Exception ex)
            {
                this.OnThreadException(new BackgroundLoopExceptionEventArgs(_currentState, ex));
                _currentState = BackgroundLoopState.None;
            }

            // Notify thread stop event
            try { _threadStopSynchronizeObject?.Release(); }
            catch (Exception)
            {
                // ignored
            }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        /// <summary>
        /// Synchronization object for threads within <see cref="BackgroundLoop"/> class.
        /// </summary>
        internal class BackgroundLoopSynchronizationContext : SynchronizationContext
        {
            private BackgroundLoop _owner;

            /// <summary>
            /// Initializes a new instance of the <see cref="BackgroundLoopSynchronizationContext"/> class.
            /// </summary>
            /// <param name="owner">The owner of this context.</param>
            public BackgroundLoopSynchronizationContext(BackgroundLoop owner)
            {
                _owner = owner;
            }

            /// <summary>
            /// When overridden in a derived class, dispatches an asynchronous message to a synchronization context.
            /// </summary>
            /// <param name="d">The <see cref="T:System.Threading.SendOrPostCallback"/> delegate to call.</param>
            /// <param name="state">The object passed to the delegate.</param>
            public override void Post(SendOrPostCallback d, object? state)
            {
                _owner.InvokeAsync(() => d(state));
            }

            /// <summary>
            /// When overridden in a derived class, dispatches a synchronous message to a synchronization context.
            /// </summary>
            /// <param name="d">The <see cref="T:System.Threading.SendOrPostCallback"/> delegate to call.</param>
            /// <param name="state">The object passed to the delegate.</param>
            public override void Send(SendOrPostCallback d, object? state)
            {
                throw new InvalidOperationException($"Synchronous messages not supported on {nameof(BackgroundLoop)}!");
            }
        }
    }
}