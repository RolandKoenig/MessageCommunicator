using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ReactiveUI;

namespace TcpCommunicator.TestGui
{
    public class ErrorDialog : OwnWindow<ErrorDialogViewModel>
    {
        public ErrorDialog()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            // Quick fix because of layout errors when showing the dialog
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += (sender, args) =>
            {
                try
                {
                    this.Width++;
                }
                catch (Exception)
                {
                    // Ignore exception
                    // This would only skip a layout pass
                }
                finally
                {
                    timer.Stop();
                }
            };
            timer.Start();
        }
    }
}
