using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using Avalonia.Controls;
using FirLib.Core.Patterns.Mvvm;

namespace MessageCommunicator.TestGui
{
    public class OwnViewServiceContainer : ViewServiceContainer
    {
        public IControl OwnerControl { get; }

        public bool IsObserving => this._observerDisposable != null;

        private ObserverStopAdapter? _observerDisposable;
        private OwnViewModelBase? _viewModel;

        public OwnViewServiceContainer(IControl owner)
            : base((IViewServiceHost)owner)
        {
            this.OwnerControl = owner;
        }

        public void StartObserving(CompositeDisposable disposables, OwnViewModelBase? viewModel)
        {
            if (_observerDisposable != null)
            {
                throw new InvalidOperationException("This instance is already registered on the view!");
            }

            _viewModel = viewModel;
            if (_viewModel != null)
            {
                _viewModel.ViewServiceRequest += this.OnViewModel_ViewServiceRequest;
            }

            _observerDisposable = new ObserverStopAdapter(this);
            disposables.Add(_observerDisposable);
        }

        private void OnViewModel_ViewServiceRequest(object? sender, ViewServiceRequestEventArgs e)
        {
            var foundViewService = this.Owner.TryFindViewService(e.ViewServiceType);
            if (foundViewService != null)
            {
                e.ViewService = foundViewService;
            }
        }

        private void StopObserving()
        {
            _observerDisposable = null;

            if (_viewModel != null)
            {
                _viewModel.ViewServiceRequest -= this.OnViewModel_ViewServiceRequest;
                _viewModel = null;
            }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        private class ObserverStopAdapter : IDisposable
        {
            private OwnViewServiceContainer _owner;

            public ObserverStopAdapter(OwnViewServiceContainer owner)
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
