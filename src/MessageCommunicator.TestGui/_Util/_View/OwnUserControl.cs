using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using FirLib.Core.Patterns.Mvvm;
using FirLib.Core.ViewServices;
using ReactiveUI;

namespace MessageCommunicator.TestGui
{
    public class OwnUserControl<T> : ReactiveUserControl<T>, IViewServiceHost
        where T : OwnViewModelBase
    {
        private OwnViewServiceContainer _viewServices;

        /// <inheritdoc />
        public ICollection<IViewService> ViewServices => _viewServices.ViewServices;

        /// <inheritdoc />
        public IViewServiceHost? ParentViewServiceHost
        {
            get
            {
                var actParent = this.Parent;
                while (actParent != null)
                {
                    if (actParent is IViewServiceHost actParentHost) { return actParentHost; }
                    actParent = actParent.Parent;
                }
                return null;
            }
        }

        /// <inheritdoc />
        public object? TryGetDefaultViewService(Type viewServiceType)
        {
            return AvaloniaDefaultViewServices.TryGetDefaultViewService(this, viewServiceType);
        }

        public OwnUserControl()
        {
            _viewServices = new OwnViewServiceContainer(this);

            this.WhenActivated(this.OnActivated);
        }

        public void RegisterViewService(IViewService viewService)
        {
            this.ViewServices.Add(viewService);
        }

        protected virtual void OnActivated(CompositeDisposable disposables)
        {
            _viewServices.StartObserving(disposables, this.ViewModel);
        }
    }
}
