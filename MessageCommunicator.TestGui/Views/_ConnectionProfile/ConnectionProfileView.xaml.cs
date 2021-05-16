using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MessageCommunicator.TestGui.Views
{
    public class ConnectionProfileView : OwnUserControl<ConnectionProfileViewModel>
    {
        private LoggingView _ctrlLoggingMessages;
        private LoggingView _ctrlLoggingDetail;

        public ConnectionProfileView()
        {
            AvaloniaXamlLoader.Load(this);

            _ctrlLoggingMessages = this.Find<LoggingView>("CtrlLoggingMessages");
            _ctrlLoggingDetail = this.Find<LoggingView>("CtrlLoggingDetail");
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if(change.Property == DataContextProperty)
            {
                var newViewModel = change.NewValue.Value as ConnectionProfileViewModel;

                _ctrlLoggingMessages.DataContext = newViewModel?.MessageLoggingViewModel;
                _ctrlLoggingDetail.DataContext = newViewModel?.DetailLoggingViewModel;
            }
        }
    }
}
