using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MessageCommunicator.TestGui.Data;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class ConnectionParametersViewModel : PropertyChangedBase
    {
        private const string CATEGORY = "Connection";

        private ConnectionParameters _connParameters;

        [Required]
        [Category(CATEGORY)]
        public string Name
        {
            get => _connParameters.Name;
            set => _connParameters.Name = value;
        }

        [Required]
        [Category(CATEGORY)]
        public string Target
        {
            get => _connParameters.Target;
            set => _connParameters.Target = value;
        }

        [Category(CATEGORY)]
        public ushort Port
        {
            get => _connParameters.Port;
            set => _connParameters.Port = value;
        }

        [Category(CATEGORY)]
        public ConnectionMode ConnectionMode
        {
            get => _connParameters.Mode;
            set
            {
                if (_connParameters.Mode != value)
                {
                    _connParameters.Mode = value;
                    this.RaisePropertyChanged(nameof(this.ConnectionMode));
                    this.RaisePropertyChanged(nameof(this.IsConfigIPEnabled));
                }
            }
        }

        [Category(CATEGORY)]
        [Range(0, int.MaxValue / 1000)]
        public int ReceiveTimeoutSec
        {
            get => _connParameters.ReceiveTimeoutSec;
            set => _connParameters.ReceiveTimeoutSec = value;
        }

        [Category(CATEGORY)]
        public MessageRecognitionMode RecognitionMode
        {
            get => _connParameters.RecognitionMode;
            set
            {
                if (_connParameters.RecognitionMode != value)
                {
                    _connParameters.RecognitionMode = value;
                    this.RaisePropertyChanged(nameof(this.RecognitionMode));

                    _connParameters.RecognizerSettings = MessageRecognizerSettingsFactory.CreateSettings(value);
                    this.RaisePropertyChanged(nameof(this.RecognizerSettings));
                }
            }
        }

        [Browsable(false)]
        public object RecognizerSettings
        {
            get => _connParameters.RecognizerSettings;
        }

        [Browsable(false)]
        public bool IsConfigIPEnabled => _connParameters.Mode == ConnectionMode.Active;

        public ConnectionParametersViewModel(ConnectionParameters connParameters)
        {
            _connParameters = connParameters;
        }
    }
}
