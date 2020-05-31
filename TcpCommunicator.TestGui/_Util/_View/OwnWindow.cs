using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace TcpCommunicator.TestGui
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
        }
    }
}
