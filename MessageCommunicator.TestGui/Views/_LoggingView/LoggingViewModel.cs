using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia;
using ReactiveUI;
using TcpCommunicator.TestGui.Logic;

namespace TcpCommunicator.TestGui.Views
{
    public class LoggingViewModel : OwnViewModelBase
    {
        private LoggingMessageWrapper? _selectedLoggingLine;

        public ObservableCollection<LoggingMessageWrapper> Logging { get; }

        public LoggingMessageWrapper? SelectedLoggingLine
        {
            get => _selectedLoggingLine;
            set
            {
                if (_selectedLoggingLine != value)
                {
                    _selectedLoggingLine = value;
                    this.RaisePropertyChanged(nameof(this.SelectedLoggingLine));
                    this.RaisePropertyChanged(nameof(this.IsAnyLineSelected));
                }
            }
        }

        public bool IsAnyLineSelected => _selectedLoggingLine != null;

        public ReactiveCommand<object?, Unit> Command_CopySelectedMessages { get; }

        public LoggingViewModel(ObservableCollection<LoggingMessageWrapper> logging)
        {
            this.Logging = logging;

            this.Command_CopySelectedMessages = ReactiveCommand.CreateFromTask<object?>(async arg =>
            {
                if (_selectedLoggingLine == null) { return; }
                await Application.Current.Clipboard.SetTextAsync(_selectedLoggingLine.Message);
            });
        }
    }
}
