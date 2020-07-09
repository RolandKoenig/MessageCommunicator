using System;
using System.Collections.Generic;
using System.Text;
using TcpCommunicator.Util;

namespace TcpCommunicator.TestGui.Data
{
    internal static class MessageRecognizerSettingsFactory
    {
        public static object CreateSettings(MessageRecognitionMode recognitionMode)
        {
            switch (recognitionMode)
            {
                case MessageRecognitionMode.Default:
                    return new MessageRecognizerDefaultSettings();

                case MessageRecognitionMode.EndSymbol:
                    return new MessageRecognizerEndSymbolSettings();

                default:
                    throw new ApplicationException(StringBuffer.Format(
                        "Unknown message recognition mode: {0}", 
                        recognitionMode));
            }
        }
    }
}
