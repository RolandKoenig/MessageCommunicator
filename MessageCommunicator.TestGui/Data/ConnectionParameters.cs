using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SharpDX.Text;

namespace MessageCommunicator.TestGui.Data
{
    public class ConnectionParameters
    {
        public string Name { get; set; } = "New Profile";

        public string Target { get; set; } = "127.0.0.1";

        public ushort Port { get; set; } = 12000;

        public ConnectionMode Mode { get; set; } = ConnectionMode.Passive;

        public MessageRecognitionMode RecognitionMode { get; set; } = MessageRecognitionMode.Default;

        public object RecognizerSettings { get; set; } =
            MessageRecognizerSettingsFactory.CreateSettings(MessageRecognitionMode.Default);
    }
}
