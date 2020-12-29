
using System;
using Newtonsoft.Json;

namespace MessageCommunicator.TestGui.Data
{
    public class ConnectionParameters
    {
        public string Name { get; set; } = "New Profile";

        [JsonProperty("ByteStreamMode")]
        public ByteStreamMode ByteStreamMode { get; set; } = ByteStreamMode.Tcp;

        public IByteStreamHandlerAppSettings ByteStreamSettings { get; set; } =
            ByteStreamSettingsFactory.CreateSettings(ByteStreamMode.Tcp);

        [JsonProperty("RecognitionMode")]
        public MessageRecognitionMode RecognitionMode { get; set; } = MessageRecognitionMode.Default;

        public IMessageRecognizerAppSettings RecognizerSettings { get; set; } =
            MessageRecognizerSettingsFactory.CreateSettings(MessageRecognitionMode.Default);
    }
}
