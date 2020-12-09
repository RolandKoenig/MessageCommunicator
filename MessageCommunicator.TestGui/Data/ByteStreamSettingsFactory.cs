using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator.TestGui.Data
{
    internal static class ByteStreamSettingsFactory
    {
        public static IByteStreamHandlerAppSettings CreateSettings(ByteStreamMode byteStreamMode)
        {
            switch (byteStreamMode)
            {
                case ByteStreamMode.TCP:
                    return new ByteStreamHandlerTcpSettings();

                case ByteStreamMode.UDP:
                    return new ByteStreamHandlerUdpSettings();

                default:
                    throw new ApplicationException($"Unknown ByteStreamMode: {byteStreamMode}");
            }
        }
    }
}
