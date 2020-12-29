using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator.TestGui.Data
{
    internal static class MessageRecognizerSettingsFactory
    {
        public static IMessageRecognizerAppSettings CreateSettings(MessageRecognizerType recognizerType)
        {
            switch (recognizerType)
            {
                case MessageRecognizerType.Default:
                    return new MessageRecognizerDefaultSettings();

                case MessageRecognizerType.EndSymbol:
                    return new MessageRecognizerEndSymbolSettings();

                case MessageRecognizerType.FixedLength:
                    return new MessageRecognizerFixedLengthSettings();

                case MessageRecognizerType.FixedLengthAndEndSymbol:
                    return new MessageRecognizerFixedLengthAndEndSymbolsSettings();

                case MessageRecognizerType.StartAndEndSymbol:
                    return new MessageRecognizerStartAndEndSymbolSettings();

                case MessageRecognizerType.ByUnderlyingPackage:
                    return new MessageRecognizerByUnderlyingPackageSettings();

                default:
                    throw new ApplicationException($"Unknown message recognition mode: {recognizerType}");
            }
        }
    }
}
