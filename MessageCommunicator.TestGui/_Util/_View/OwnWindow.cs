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
        /// <inheritdoc />
        public List<object> ViewServices { get; } = new List<object>();

        public OwnWindow()
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

            Observable.FromEventPattern<CloseWindowRequestEventArgs>(this.ViewModel, nameof(this.ViewModel.CloseWindowRequest))
                .Subscribe(eArgs =>
                {
                    this.Close(eArgs.EventArgs.DialogResult);
                })
                .DisposeWith(disposables);
        }
    }
}
