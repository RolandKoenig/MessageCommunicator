using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace MessageCommunicator.TestGui
{
    public class OwnUserControlDialog<T> : ReactiveUserControl<T>, IViewServiceHost
        where T : OwnViewModelBase
    {
        private DialogHostControl? _host;
        private TaskCompletionSource<object?>? _closeCompletionSource;
        private ViewServiceContainer _viewServices;

        /// <inheritdoc />
        public ICollection<IViewService> ViewServices => _viewServices.ViewServices;

        public OwnUserControlDialog()
        {
            _viewServices = new ViewServiceContainer(this);

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
            _viewServices.ObserveForViewServiceRequest(disposables, this.ViewModel);

            Observable.FromEventPattern<CloseWindowRequestEventArgs>(this.ViewModel, nameof(this.ViewModel.CloseWindowRequest))
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
