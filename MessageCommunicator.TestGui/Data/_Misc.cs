namespace TcpCommunicator.TestGui.Data
{
    public enum ConnectionMode
    {
        Passive,

        Active
    }

    public enum MessageRecognitionMode
    {
        Default,

        EndSymbol,

        FixedLengthAndEndSymbol
    }
}
