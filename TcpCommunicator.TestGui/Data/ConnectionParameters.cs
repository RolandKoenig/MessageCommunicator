using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SharpDX.Text;

namespace TcpCommunicator.TestGui.Data
{
    public class ConnectionParameters
    {
        private const string CATEGORY = "Connection";

        [Category(CATEGORY)]
        [Required]
        public string Name { get; set; } = "New Profile";

        [Category(CATEGORY)]
        public string Target { get; set; } = "127.0.0.1";

        [Category(CATEGORY)]
        public ushort Port { get; set; } = 12000;

        [Category(CATEGORY)]
        public ConnectionMode Mode { get; set; } = ConnectionMode.Passive;

        [Category(CATEGORY)]
        public MessageRecognitionMode RecognitionMode { get; set; } = MessageRecognitionMode.Default;

        [Browsable(false)]
        public object? RecognizerSettings { get; set; }

        public static object? CreateRecognizerSettings(MessageRecognitionMode recognitionMode)
        {
            switch (recognitionMode)
            {
                case MessageRecognitionMode.Default:
                    return new RecognizerDefaultSettings();

                case MessageRecognitionMode.EndSymbol:
                    return new RecognizerEndSymbolSettings();

                default:
                    return null;
            }
        }
    }
}
