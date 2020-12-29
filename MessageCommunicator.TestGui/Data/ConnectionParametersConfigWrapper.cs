using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MessageCommunicator.TestGui.Data;

namespace MessageCommunicator.TestGui.Data
{
    public class ConnectionParametersConfigWrapper : PropertyChangedBase
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
        [Description("The ByteStreamHandler is responsible for sending and receiving binary packages.")]
        public ByteStreamMode ByteStreamHandler
        {
            get => _connParameters.ByteStreamMode;
            set
            {
                if (_connParameters.ByteStreamMode != value)
                {
                    _connParameters.ByteStreamMode = value;
                    this.RaisePropertyChanged(nameof(this.ByteStreamHandler));

                    _connParameters.ByteStreamSettings = ByteStreamSettingsFactory.CreateSettings(value);
                    this.RaisePropertyChanged(nameof(this.ByteStreamHandlerSettings));
                }
            }
        }

        [Browsable(false)]
        public object ByteStreamHandlerSettings => _connParameters.ByteStreamSettings;

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
                    this.RaisePropertyChanged(nameof(this.MessageRecognizerSettings));
                }
            }
        }

        [Browsable(false)]
        public object MessageRecognizerSettings
        {
            get => _connParameters.RecognizerSettings;
        }
        
        public ConnectionParametersConfigWrapper(ConnectionParameters connParameters)
        {
            _connParameters = connParameters;
        }
    }
}
