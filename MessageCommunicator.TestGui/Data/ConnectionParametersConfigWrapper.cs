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
        [Description("A unique profile name.")]
        public string Name
        {
            get => _connParameters.Name;
            set => _connParameters.Name = value;
        }

        [Category(CATEGORY)]
        [HelpFileLink("ByteStreamHandler")]
        [DisplayName("ByteStreamHandler")]
        [Description("The ByteStreamHandler is responsible for sending and receiving binary packages.")]
        public ByteStreamHandlerType ByteStreamHandlerType
        {
            get => _connParameters.ByteStreamHandlerType;
            set
            {
                if (_connParameters.ByteStreamHandlerType != value)
                {
                    _connParameters.ByteStreamHandlerType = value;
                    this.RaisePropertyChanged(nameof(this.ByteStreamHandlerType));

                    _connParameters.ByteStreamHandlerSettings = ByteStreamSettingsFactory.CreateSettings(value);
                    this.RaisePropertyChanged(nameof(this.ByteStreamHandlerSettings));
                }
            }
        }

        [Browsable(false)]
        public object ByteStreamHandlerSettings => _connParameters.ByteStreamHandlerSettings;

        [Category(CATEGORY)]
        [HelpFileLink("MessageRecognizer")]
        [Description("The MessageRecognizer is responsible to wrap outgoing messages with the right information like " +
                     "common header or end-symbols.")]
        public MessageRecognizerType MessageRecognizerType
        {
            get => _connParameters.MessageRecognizerType;
            set
            {
                if (_connParameters.MessageRecognizerType != value)
                {
                    _connParameters.MessageRecognizerType = value;
                    this.RaisePropertyChanged(nameof(this.MessageRecognizerType));

                    _connParameters.MessageRecognizerSettings = MessageRecognizerSettingsFactory.CreateSettings(value);
                    this.RaisePropertyChanged(nameof(this.MessageRecognizerSettings));
                }
            }
        }

        [Browsable(false)]
        public object MessageRecognizerSettings
        {
            get => _connParameters.MessageRecognizerSettings;
        }
        
        public ConnectionParametersConfigWrapper(ConnectionParameters connParameters)
        {
            _connParameters = connParameters;
        }
    }
}
