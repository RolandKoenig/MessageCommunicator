using System.Collections.Generic;
using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace MessageCommunicator.TestGui
{
    public class OwnUserControl<T> : ReactiveUserControl<T>, IViewServiceHost
        where T : OwnViewModelBase
    {
        /// <inheritdoc />
        public List<object> ViewServices { get; } = new List<object>();

        public OwnUserControl()
        {
            this.WhenActivated(this.OnActivated);
        }

        public void RegisterViewService(object viewService)
        {
            this.ViewServices.Add(viewService);
        }

        protected virtual void OnActivated(CompositeDisposable disposables)
        {
            this.ObserveForViewServiceRequest(disposables, this.ViewModel);
        }
    }
}
