using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.ReactiveUI;
using FirLib.Core.Avalonia.Controls;
using FirLib.Core.Patterns.Mvvm;
using FirLib.Core.ViewServices;
using ReactiveUI;

namespace MessageCommunicator.TestGui
{
    public class OwnUserControlDialog<T> : ReactiveUserControl<T>, IViewServiceHost
        where T : OwnViewModelBase
    {
        private DialogHostControl? _host;
        private TaskCompletionSource<object?>? _closeCompletionSource;
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

        public OwnUserControlDialog()
        {
            _viewServices = new OwnViewServiceContainer(this);

            this.WhenActivated(this.OnActivated);
        }

        public Task<object?> ShowControlDialogAsync(DialogHostControl host, string headerText)
        {
            if (_closeCompletionSource != null)
            {
                throw new ApplicationException($"Unable to show {this.GetType().FullName} because it is already shown!");
            }
            _closeCompletionSource = new TaskCompletionSource<object?>();

            _host = host;
            _host.ShowDialog(this, headerText);

            return _closeCompletionSource.Task;
        }

        public void RegisterViewService(IViewService viewService)
        {
            this.ViewServices.Add(viewService);
        }

        protected virtual void OnActivated(CompositeDisposable disposables)
        {
            _viewServices.StartObserving(disposables, this.ViewModel);

            Observable.FromEventPattern<EventHandler<CloseWindowRequestEventArgs>, CloseWindowRequestEventArgs>(
                    eHandler => this.ViewModel!.CloseWindowRequest += eHandler,
                    eHandler => this.ViewModel!.CloseWindowRequest -= eHandler)
                .Subscribe(eArgs =>
                {
                    if (_closeCompletionSource == null) { return; }

                    _host!.CloseDialog();
                    _host = null;

                    var complSource = _closeCompletionSource;
                    _closeCompletionSource = null;
                    complSource.SetResult(eArgs.EventArgs.DialogResult);
                })
                .DisposeWith(disposables);
        }
    }
}
