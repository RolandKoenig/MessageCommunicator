using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.Samples
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // Create and start receive channel
            var passiveTcpChannel = new MessageChannel(
                new TcpPassiveByteSteamHandlerSettings(IPAddress.Loopback, 12000),
                new EndSymbolsMessageRecognizerSettings(Encoding.UTF8, "##"),
                (message) =>
                {
                    Console.WriteLine($"Received message on passive channel: {message}");
                });
            await passiveTcpChannel.StartAsync();

            // Create and start send channel
            var activeTcpChannel = new MessageChannel(
                new TcpActiveByteSteamHandlerSettings("127.0.0.1", 12000), 
                new EndSymbolsMessageRecognizerSettings(Encoding.UTF8, "##"),
                (message) =>
                {
                    Console.WriteLine($"Received message on active channel: {message}");
                });
            await activeTcpChannel.StartAsync();

            // Wait until both channels are connected
            while (passiveTcpChannel.State != ConnectionState.Connected)
            {
                await Task.Delay(500);
            }

            // Send some messages (active -> passive)
            await activeTcpChannel.SendAsync(new Message("Message 1 from active to passive..."));
            await activeTcpChannel.SendAsync(new Message("Message 2 from active to passive..."));
            await activeTcpChannel.SendAsync(new Message("Message 3 from active to passive..."));

            // Send some messages (passive -> active)
            await passiveTcpChannel.SendAsync(new Message("Message 1 from active to passive..."));
            await passiveTcpChannel.SendAsync(new Message("Message 2 from active to passive..."));
            await passiveTcpChannel.SendAsync(new Message("Message 3 from active to passive..."));

            // Wait 
            Console.ReadLine();

            // Stop both channels
            await activeTcpChannel.StopAsync();
            await passiveTcpChannel.StopAsync();
        }
    }
}
