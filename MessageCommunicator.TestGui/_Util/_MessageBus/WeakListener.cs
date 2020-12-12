using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace MessageCommunicator.TestGui
{
    public static class WeakListener
    {
        public static void ListenWeak<T>(this IMessageBus messageBus, IWeakMessageTarget<T> handler)
        {
            WeakReference weakHandler = new WeakReference(handler);

            IDisposable? disposable = null;
            disposable = messageBus.Listen<T>().Subscribe(eArgs =>
            {
                if (disposable == null) { return; }

                if (!(weakHandler.Target is IWeakMessageTarget<T> target))
                {
                    disposable?.Dispose();
                    disposable = null;
                    return;
                }

                target.OnMessage(eArgs);
            });
        }
    }
}
