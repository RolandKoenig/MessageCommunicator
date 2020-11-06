using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using Avalonia.Controls;

namespace MessageCommunicator.TestGui
{
    public class ViewServiceContainer
    {
        public ObservableCollection<IViewService> ViewServices { get; } = new ObservableCollection<IViewService>();

        public IControl Owner { get; }

        public ViewServiceContainer(IControl owner)
        {
            this.Owner = owner;

            this.ViewServices.CollectionChanged += this.OnViewServices_CollectionChanged;
        }

        internal void ObserveForViewServiceRequest(CompositeDisposable disposables, OwnViewModelBase? viewModel)
        {
            if (viewModel == null) { return; }

            Observable.FromEventPattern<ViewServiceRequestEventArgs>(viewModel, nameof(OwnViewModelBase.ViewServiceRequest))
                .Subscribe(onNext =>
                {
                    var eArgs = onNext.EventArgs;
                    var foundViewService = this.FindViewService(eArgs.ViewServiceType);
                    if (foundViewService != null)
                    {
                        eArgs.ViewService = foundViewService;
                    }
                })
                .DisposeWith(disposables);
        }

        internal object? FindViewService(Type viewServiceType)
        {
            var actParent = (IControl?)this.Owner;
            object? result = null;
            while (actParent != null)
            {
                if (actParent is IViewServiceHost viewServiceHost)
                {
                    foreach (var actViewService in viewServiceHost.ViewServices)
                    {
                        if(actViewService == null){ continue; }

                        // ReSharper disable once UseMethodIsInstanceOfType
                        if(!viewServiceType.IsAssignableFrom(actViewService.GetType())){ continue; }

                        result = actViewService;
                    }
                }

                actParent = actParent.Parent;
            }
            return result;
        }

        private void OnViewServices_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
        }
    }
}
