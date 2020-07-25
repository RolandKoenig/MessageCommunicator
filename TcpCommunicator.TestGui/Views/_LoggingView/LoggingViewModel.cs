using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                }
            }
        }

        public LoggingViewModel(ObservableCollection<LoggingMessageWrapper> logging)
        {
            this.Logging = logging;
        }
    }
}
