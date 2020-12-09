namespace MessageCommunicator.TestGui.Data
{
    public enum ByteStreamMode
    {
        TCP = 0,

        UDP = 1
    }

    public enum ConnectionMode
    {
        Passive = 0,

        Active = 1
    }

    public enum MessageRecognitionMode
    {
        Default = 0,

        EndSymbol = 1,

        FixedLength = 4,

        FixedLengthAndEndSymbol = 2,

        StartAndEndSymbol = 3,

        ByUnderlyingPackage = 5
    }
}
