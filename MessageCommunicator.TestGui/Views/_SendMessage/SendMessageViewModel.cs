using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MessageCommunicator.TestGui.Logic;
using MessageCommunicator.Util;
using ReactiveUI;

namespace MessageCommunicator.TestGui.Views
{
    public class SendMessageViewModel : OwnViewModelBase
    {
        private IConnectionProfile? _currentConnectionProfile;
        private SendFormattingMode _sendFormattingMode;
        private string _currentMessage;

        public IConnectionProfile? CurrentConnectionProfile
        {
            get => _currentConnectionProfile;
            set
            {
                if (_currentConnectionProfile != value)
                {
                    _currentConnectionProfile = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public SendFormattingMode SendFormattingMode
        {
            get => _sendFormattingMode;
            set
            {
                if (_sendFormattingMode != value)
                {
                    _sendFormattingMode = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string CurrentMessage
        {
            get => _currentMessage;
            set
            {
                if (_currentMessage != value)
                {
                    _currentMessage = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public SendFormattingMode[] SendFormattingModeList => (SendFormattingMode[])Enum.GetValues(typeof(SendFormattingMode));

        public ReactiveCommand<string?, Unit> Command_SendMessage { get; }

        public SendMessageViewModel()
        {
            _sendFormattingMode = SendFormattingMode.Plain;
            _currentMessage = string.Empty;

            this.Command_SendMessage = ReactiveCommand.CreateFromTask<string?>(
                this.OnCommand_SendMessage_Execute);
        }

        private async Task OnCommand_SendMessage_Execute(string? message)
        {
            if (_currentConnectionProfile == null) { return; }
            var connProfile = _currentConnectionProfile;

            try
            {
                message ??= string.Empty;

                switch (this.SendFormattingMode)
                {
                    case SendFormattingMode.Plain:
                        break;

                    case SendFormattingMode.Escaped:
                        message = Regex.Unescape(message);
                        break;

                    case SendFormattingMode.BinaryHex:
                        var encoding = Encoding.GetEncoding(connProfile.Parameters.RecognizerSettings.Encoding);
                        message = encoding.GetString(HexFormatUtil.ToByteArray(message));
                        break;

                    default:
                        throw new InvalidOperationException(
                            $"Unhandled {nameof(Views.SendFormattingMode)} {this.SendFormattingMode}!");
                }

                await connProfile.SendMessageAsync(message);
            }
            catch (Exception e)
            {
                CommonErrorHandling.Current.ShowErrorDialog(e);
            }
        }
    }
}
