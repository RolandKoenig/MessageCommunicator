using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using Avalonia.Controls;
using ReactiveUI;

namespace TcpCommunicator.TestGui
{
    internal static class OwnViewExtensions
    {
        internal static void ObserveForViewServiceRequest(this IControl view, CompositeDisposable disposables, OwnViewModelBase? viewModel)
        {
            if (viewModel == null) { return; }

            Observable.FromEventPattern<ViewServiceRequestEventArgs>(viewModel, nameof(OwnViewModelBase.ViewServiceRequest))
                .Subscribe((onNext) =>
                {
                    var eArgs = onNext.EventArgs;
                    var foundViewService = view.FindViewService(eArgs.ViewServiceType);
                    if (foundViewService != null)
                    {
                        eArgs.ViewService = foundViewService;
                    }
                })
                .DisposeWith(disposables);
        }

        internal static object? FindViewService(this IControl view, Type viewServiceType)
        {
            var actParent = view;
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
    }
}
