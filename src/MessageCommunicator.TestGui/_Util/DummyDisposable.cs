﻿using System;

namespace MessageCommunicator.TestGui
{
    internal class DummyDisposable : IDisposable
    {
        private Action _disposeAction;

        public DummyDisposable(Action disposeAction)
        {
            _disposeAction = disposeAction;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _disposeAction();
        }
    }
}
