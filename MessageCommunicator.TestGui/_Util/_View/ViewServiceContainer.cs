using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Text;
using Avalonia.Controls;

namespace MessageCommunicator.TestGui
{
    public class ViewServiceContainer
    {
        public ObservableCollection<IViewService> ViewServices { get; } = new ObservableCollection<IViewService>();
        public IControl Owner { get; }

        public bool IsObserving => this._observerDisposable != null;

        private ObserverStopAdapter? _observerDisposable;
        private OwnViewModelBase? _observedViewModel;

        public ViewServiceContainer(IControl owner)
        {
            this.Owner = owner;

            this.ViewServices.CollectionChanged += this.OnViewServices_CollectionChanged;
        }

        public void StartObserving(CompositeDisposable disposables, OwnViewModelBase? viewModel)
        {
            if (viewModel == null) { return; }
            if (_observerDisposable != null)
            {
                throw new InvalidOperationException("This instance is already registered on the view!");
            }

            _observedViewModel = viewModel;
            _observedViewModel.ViewServiceRequest += this.OnViewServiceRequest;

            _observerDisposable = new ObserverStopAdapter(this);
            disposables.Add(_observerDisposable);

            foreach (var actViewServices in this.ViewServices)
            {
                actViewServices.ViewServiceRequest += this.OnViewServiceRequest;
            }
        }

        private void StopObserving()
        {
            _observerDisposable = null;

            if (_observedViewModel != null)
            {
                _observedViewModel.ViewServiceRequest -= this.OnViewServiceRequest;
                _observedViewModel = null;
            }

            foreach (var actRegisteredViewService in this.ViewServices)
            {
                actRegisteredViewService.ViewServiceRequest -= this.OnViewServiceRequest;
            }
        }

        private object? FindViewService(Type viewServiceType)
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
            if (_observerDisposable == null) { return; }

            foreach (IViewService? actNewItem in e.NewItems)
            {
                if(actNewItem == null){ continue; }

                actNewItem.ViewServiceRequest += this.OnViewServiceRequest;
            }
            foreach (IViewService? actOldItem in e.OldItems)
            {
                if(actOldItem == null){ continue; }

                actOldItem.ViewServiceRequest -= this.OnViewServiceRequest;
            }
        }

        private void OnViewServiceRequest(object? sender, ViewServiceRequestEventArgs e)
        {
            var foundViewService = this.FindViewService(e.ViewServiceType);
            if (foundViewService != null)
            {
                e.ViewService = foundViewService;
            }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        private class ObserverStopAdapter : IDisposable
        {
            private ViewServiceContainer _owner;

            public ObserverStopAdapter(ViewServiceContainer owner)
            {
                _owner = owner;
            }

            /// <inheritdoc />
            public void Dispose()
            {
                _owner.StopObserving();
            }
        }
    }
}
