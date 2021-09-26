using System;

namespace FirLib.Core.Patterns.BackgroundLoops
{
    /// <summary>
    /// Enumeration containing all possible states of a BackgroundLoop object.
    /// </summary>
    public enum BackgroundLoopState
    {
        /// <summary>
        /// There is no thread created at the moment.
        /// </summary>
        None,

        /// <summary>
        /// The thread is starting.
        /// </summary>
        Starting,

        /// <summary>
        /// The thread is running.
        /// </summary>
        Running,

        /// <summary>
        /// The thread is stopping.
        /// </summary>
        Stopping
    }

    public readonly struct BackgroundLoopTickEventArgs
    {
        public readonly long ElapsedTicks;

        public BackgroundLoopTickEventArgs(long elapsedTicks)
        {
            this.ElapsedTicks = elapsedTicks;
        }
    }

    public class BackgroundLoopExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the occurred exception.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets current state of the thread.
        /// </summary>
        public BackgroundLoopState State { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundLoopExceptionEventArgs"/> class.
        /// </summary>
        /// <param name="threadState">The current state of the <see cref="BackgroundLoop"/>.</param>
        /// <param name="innerException">The inner exception.</param>
        public BackgroundLoopExceptionEventArgs(BackgroundLoopState threadState, Exception innerException)
        {
            this.Exception = innerException;
            this.State = threadState;
        }
    }
}