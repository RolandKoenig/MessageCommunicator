using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace MessageCommunicator.TestGui
{
    public class OwnWindow<T> : ReactiveWindow<T>, IViewServiceHost
        where T : OwnViewModelBase
    {
        private ViewServiceContainer _viewServices;

        /// <inheritdoc />
        public ICollection<IViewService> ViewServices => _viewServices.ViewServices;

        public OwnWindow()
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
            _viewServices.StartObserving(disposables, this.ViewModel);

            Observable.FromEventPattern<EventHandler<CloseWindowRequestEventArgs>, CloseWindowRequestEventArgs>(
                    eventHandler => this.ViewModel.CloseWindowRequest += eventHandler,
                    eventHandler => this.ViewModel.CloseWindowRequest -= eventHandler)
                .Subscribe(eArgs =>
                {
                    this.Close(eArgs.EventArgs.DialogResult);
                })
                .DisposeWith(disposables);
        }
    }
}
