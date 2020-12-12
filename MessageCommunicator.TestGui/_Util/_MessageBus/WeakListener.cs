using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace MessageCommunicator.TestGui
{
    public static class WeakListener
    {
        public static IDisposable ListenWeak<T>(this IMessageBus messageBus, IWeakMessageTarget<T> handler)
        {
            WeakReference weakHandler = new WeakReference(handler);

            IDisposable? disposable = null;
            disposable = messageBus.Listen<T>().Subscribe(eArgs =>
            {
                if (disposable == null) { return; }

                if (!(weakHandler.Target is IWeakMessageTarget<T> target))
                {
                    disposable.Dispose();
                    disposable = null;
                    return;
                }

                target.OnMessage(eArgs);
            });

            return disposable;
        }
    }
}
