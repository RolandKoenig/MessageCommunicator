using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
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

        [Category(CATEGORY)]
        [HelpFileLink("ByteStreamHandler")]
        [DisplayName("ByteStreamHandler")]
        public ByteStreamMode ByteStreamMode
        {
            get => _connParameters.ByteStreamMode;
            set
            {
                if (_connParameters.ByteStreamMode != value)
                {
                    _connParameters.ByteStreamMode = value;
                    this.RaisePropertyChanged(nameof(this.ByteStreamMode));

                    _connParameters.ByteStreamSettings = ByteStreamSettingsFactory.CreateSettings(value);
                    this.RaisePropertyChanged(nameof(this.ByteStreamSettings));
                }
            }
        }

        [Browsable(false)]
        public object ByteStreamSettings => _connParameters.ByteStreamSettings;

        [Category(CATEGORY)]
        [HelpFileLink("MessageRecognizer")]
        public MessageRecognitionMode MessageRecognizer
        {
            get => _connParameters.RecognitionMode;
            set
            {
                if (_connParameters.RecognitionMode != value)
                {
                    _connParameters.RecognitionMode = value;
                    this.RaisePropertyChanged(nameof(this.MessageRecognizer));

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
        
        public ConnectionParametersViewModel(ConnectionParameters connParameters)
        {
            _connParameters = connParameters;
        }
    }
}
