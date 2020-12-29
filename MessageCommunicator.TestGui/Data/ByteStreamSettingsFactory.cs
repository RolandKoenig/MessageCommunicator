using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator.TestGui.Data
{
    internal static class ByteStreamSettingsFactory
    {
        public static IByteStreamHandlerAppSettings CreateSettings(ByteStreamHandlerType byteStreamHandlerType)
        {
            switch (byteStreamHandlerType)
            {
                case ByteStreamHandlerType.Tcp:
                    return new ByteStreamHandlerTcpSettings();

                case ByteStreamHandlerType.Udp:
                    return new ByteStreamHandlerUdpSettings();

                case ByteStreamHandlerType.SerialPort:
                    return new ByteStreamHandlerSerialPortSettings();

                default:
                    throw new ApplicationException($"Unknown ByteStreamMode: {byteStreamHandlerType}");
            }
        }
    }
}
