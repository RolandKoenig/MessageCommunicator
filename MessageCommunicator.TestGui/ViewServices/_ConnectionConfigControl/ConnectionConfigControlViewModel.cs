using System;
using System.Collections.Generic;
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
        private ConnectionParameters? _originalConnectionParameters;
        private IEnumerable<ConnectionParameters> _allConnectionParameters;

        public ConnectionParameters Model { get; }

        public ConnectionParametersConfigWrapper ModelInteractive { get; }

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

        public ConnectionConfigControlViewModel(ConnectionParameters? parameters, IEnumerable<ConnectionParameters> allConnectionParameters)
        {
            this.Model = parameters != null ? parameters.DeepClone() : new ConnectionParameters();
            this.ModelInteractive = new ConnectionParametersConfigWrapper(this.Model);

            this.Command_OK = ReactiveCommand.Create<object?>(this.OnCommandOK);
            this.Command_Cancel = ReactiveCommand.Create<object?>(
                arg => this.CloseWindow(null));

            _originalConnectionParameters = parameters;
            _allConnectionParameters = allConnectionParameters;
        }

        private void OnCommandOK(object? arg)
        {
            var model = this.Model;

            // Perform validation
            this.ValidationError = string.Empty;
            try
            {
                Validator.ValidateObject(this.ModelInteractive, new ValidationContext(this.ModelInteractive), true);
                Validator.ValidateObject(model.ByteStreamSettings, new ValidationContext(model.ByteStreamSettings), true);
                Validator.ValidateObject(model.RecognizerSettings, new ValidationContext(model.RecognizerSettings), true);
            }
            catch (ValidationException e)
            {
                this.ValidationError = e.Message;
                return;
            }

            // Check for duplicate name
            foreach (var actOtherConnection in _allConnectionParameters)
            {
                if(actOtherConnection == _originalConnectionParameters){ continue; }

                if (actOtherConnection.Name.Trim() == this.Model.Name.Trim())
                {
                    this.ValidationError = $"The name '{actOtherConnection.Name}' is already in use!";
                    return;
                }
            }

            this.CloseWindow(model);
        }
    }
}
