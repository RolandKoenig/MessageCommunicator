using System;
using System.Reactive.Disposables;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace TcpCommunicator.TestGui
{
    public class MainWindow : OwnWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);

            this.ViewServices.Add(new ConnectionConfigViewService(this));

            this.ViewModel = new MainWindowViewModel();
            this.DataContext = this.ViewModel;

#if DEBUG
            this.AttachDevTools();
#endif
        }
    }
}
