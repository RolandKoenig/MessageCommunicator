namespace TcpCommunicator
{
    internal class Boxed<T> where T : struct
    {
        public T Value { get; set; }
    }
}
