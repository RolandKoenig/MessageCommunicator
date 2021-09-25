
using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace MessageCommunicator.TestGui.Data
{
    public class ConnectionParameters
    {
        public string Name { get; set; } = "New Profile";

        [JsonProperty("ByteStreamMode")]
        public ByteStreamHandlerType ByteStreamHandlerType { get; set; } = ByteStreamHandlerType.Tcp;

        [JsonProperty("ByteStreamSettings")]
        public IByteStreamHandlerAppSettings ByteStreamHandlerSettings { get; set; } =
            ByteStreamSettingsFactory.CreateSettings(ByteStreamHandlerType.Tcp);

        [JsonProperty("RecognitionMode")]
        public MessageRecognizerType MessageRecognizerType { get; set; } = MessageRecognizerType.Default;

        [JsonProperty("RecognizerSettings")]
        public IMessageRecognizerAppSettings MessageRecognizerSettings { get; set; } =
            MessageRecognizerSettingsFactory.CreateSettings(MessageRecognizerType.Default);
    }
}
