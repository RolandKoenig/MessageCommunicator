using System;
using System.Collections.Generic;
using System.Text;
using MessageCommunicator.Util;

namespace MessageCommunicator.TestGui.Data
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

                case MessageRecognitionMode.FixedLengthAndEndSymbol:
                    return new MessageRecognizerFixedLengthAndEndSymbolsSettings();

                default:
                    throw new ApplicationException($"Unknown message recognition mode: {recognitionMode}");
            }
        }
    }
}
