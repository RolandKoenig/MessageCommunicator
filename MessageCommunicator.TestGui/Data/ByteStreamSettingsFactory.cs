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
                case ByteStreamMode.Tcp:
                    return new ByteStreamHandlerTcpSettings();

                case ByteStreamMode.Udp:
                    return new ByteStreamHandlerUdpSettings();

                case ByteStreamMode.SerialPort:
                    return new ByteStreamHandlerSerialPortSettings();

                default:
                    throw new ApplicationException($"Unknown ByteStreamMode: {byteStreamMode}");
            }
        }
    }
}
