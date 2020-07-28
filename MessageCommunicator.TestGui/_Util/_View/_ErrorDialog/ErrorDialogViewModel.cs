using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using DynamicData.Kernel;
using ReactiveUI;

namespace MessageCommunicator.TestGui
{
    public class ErrorDialogViewModel : OwnViewModelBase
    {
        public string Title { get; set; } = string.Empty;

        public string MainMessage { get; set; } = string.Empty;

        public string DetailMessage { get; set; } = string.Empty;

        public ReactiveCommand<object?, Unit> Command_OK { get; }

        private ErrorDialogViewModel()
        {
            this.Command_OK = ReactiveCommand.Create<object?>(arg => this.CloseWindow(null));
        }

        public ErrorDialogViewModel(string title, string mainMessage)
            : this()
        {
            this.Title = title;
            this.MainMessage = mainMessage;
        }

        public ErrorDialogViewModel(Exception unhandledException)
            : this()
        {
            this.Title = "Unhandled exception";
            this.MainMessage = unhandledException.Message;
            this.DetailMessage = unhandledException.ToString();
        }
    }
}
