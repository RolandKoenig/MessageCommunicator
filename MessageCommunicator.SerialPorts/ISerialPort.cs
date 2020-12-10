using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace MessageCommunicator.SerialPorts
{
    public interface ISerialPort : IDisposable
    {
        bool IsOpen { get; }

        string PortName { get; set; }

        int BaudRate { get; set; }

        int DataBits { get; set; }

        StopBits StopBits { get; set; }

        Handshake Handshake { get; set; }

        int ReadTimeout { get; set; }

        int WriteTimeout { get; set; }

        Parity Parity { get; set; }

        bool DtrEnable { get; set; }

        bool RtsEnable { get; set; }

        int BytesToRead { get; }

        event SerialDataReceivedEventHandler DataReceived;

        void Open();

        void Close();

        void Write(byte[] buffer, int offset, int length);

        int Read(byte[] buffer, int offset, int length);
    }
}
