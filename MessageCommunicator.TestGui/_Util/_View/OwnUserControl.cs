using System.Collections.Generic;
using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace MessageCommunicator.TestGui
{
    public class OwnUserControl<T> : ReactiveUserControl<T>, IViewServiceHost
        where T : OwnViewModelBase
    {
        private ViewServiceContainer _viewServices;

        /// <inheritdoc />
        public ICollection<IViewService> ViewServices => _viewServices.ViewServices;

        public OwnUserControl()
        {
            _viewServices = new ViewServiceContainer(this);

            this.WhenActivated(this.OnActivated);
        }

        public void RegisterViewService(IViewService viewService)
        {
            this.ViewServices.Add(viewService);
        }

        protected virtual void OnActivated(CompositeDisposable disposables)
        {
            _viewServices.ObserveForViewServiceRequest(disposables, this.ViewModel);
        }
    }
}
