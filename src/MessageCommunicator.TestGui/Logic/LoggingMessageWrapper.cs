namespace MessageCommunicator.TestGui.Logic
{
    public class LoggingMessageWrapper
    {
        private LoggingMessage _message;

        public string TimeStamp => _message.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff");

        public string MessageType => _message.MessageType.ToString();

        public string MetaData => _message.MetaData;

        public string Message => _message.Message;

        public LoggingMessageWrapper(LoggingMessage message)
        {
            _message = message;
        }
    }
}