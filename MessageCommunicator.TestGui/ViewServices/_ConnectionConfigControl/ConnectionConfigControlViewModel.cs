using System;
using System.ComponentModel.DataAnnotations;
using System.Reactive;
using Force.DeepCloner;
using MessageCommunicator.TestGui.Data;
using ReactiveUI;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class ConnectionConfigControlViewModel : OwnViewModelBase
    {
        private string _validationError = string.Empty;

        public ConnectionParameters Model { get; }

        public ConnectionParametersViewModel ModelInteractive { get; }

        public ReactiveCommand<object?, Unit> Command_OK { get; }

        public ReactiveCommand<object?, Unit> Command_Cancel { get; }

        public ConnectionMode[] ConnectionModes => (ConnectionMode[])Enum.GetValues(typeof(ConnectionMode));

        public string ValidationError
        {
            get => _validationError;
            set
            {
                if (_validationError != value)
                {
                    _validationError = value;
                    this.RaisePropertyChanged(nameof(this.ValidationError));
                    this.RaisePropertyChanged(nameof(this.IsValidationErrorVisible));
                }
            }
        }

        public bool IsValidationErrorVisible => !string.IsNullOrEmpty(this.ValidationError);

        public ConnectionConfigControlViewModel(ConnectionParameters? parameters = null)
        {
            this.Model = parameters != null ? parameters.DeepClone() : new ConnectionParameters();
            this.ModelInteractive = new ConnectionParametersViewModel(this.Model);

            this.Command_OK = ReactiveCommand.Create<object?>(this.OnCommandOK);
            this.Command_Cancel = ReactiveCommand.Create<object?>(
                arg => this.CloseWindow(null));
        }

        private void OnCommandOK(object? arg)
        {
            var model = this.Model;

            // Perform validation
            this.ValidationError = string.Empty;
            try
            {
                Validator.ValidateObject(model, new ValidationContext(model), true);
                Validator.ValidateObject(model.RecognizerSettings, new ValidationContext(model.RecognizerSettings), true);
            }
            catch (ValidationException e)
            {
                this.ValidationError = e.Message;
                return;
            }

            this.CloseWindow(model);
        }
    }
}
