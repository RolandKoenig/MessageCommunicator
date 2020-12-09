using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessageCommunicator.Tests.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageCommunicator.Tests
{
    [TestClass]
    public class MessageChannelTests
    {
        [TestMethod]
        public async Task Check_SimpleUdpConnection()
        {
            var testingPort1 = TestUtility.GetFreeTcpPort();
            var testingPort2 = TestUtility.GetFreeTcpPort();

            var receiveTaskOnPassiveChannel = new TaskCompletionSource<string>();
            var receiveTaskOnActiveChannel = new TaskCompletionSource<string>();

            // Define channels
            var passiveChannel = new MessageChannel(
                new UdpByteStreamHandlerSettings(testingPort1,IPAddress.Loopback, testingPort2),
                new DefaultMessageRecognizerSettings(Encoding.UTF8),
                (msg) =>
                {
                    receiveTaskOnPassiveChannel.SetResult(msg.ToString());
                    msg.ReturnToPool();
                });
            var activeChannel = new MessageChannel(
                new UdpByteStreamHandlerSettings(testingPort2,IPAddress.Loopback, testingPort1),
                new DefaultMessageRecognizerSettings(Encoding.UTF8),
                (msg) =>
                {
                    receiveTaskOnActiveChannel.SetResult(msg.ToString());
                    msg.ReturnToPool();
                });
            try
            {
                // Start both channels
                await Task.WhenAll(
                    passiveChannel.StartAsync(),
                    activeChannel.StartAsync());

                // Wait for connection
                var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5.0));
                await Task.WhenAll(
                    passiveChannel.WaitForConnectionAsync(timeoutTokenSource.Token),
                    activeChannel.WaitForConnectionAsync(timeoutTokenSource.Token));
                Assert.IsTrue(passiveChannel.State == ConnectionState.Connected);
                Assert.IsTrue(activeChannel.State == ConnectionState.Connected);

                // Send/Receive some messages in both directions
                await passiveChannel.SendAsync("Message from passive endpoint");
                await activeChannel.SendAsync("Message from active endpoint");

                // Check for received messages
                var receivedOnPassiveChannel = await receiveTaskOnPassiveChannel.Task;
                var receivedOnActiveChannel = await receiveTaskOnActiveChannel.Task;
                Assert.IsTrue(receivedOnPassiveChannel == "Message from active endpoint");
                Assert.IsTrue(receivedOnActiveChannel == "Message from passive endpoint");
            }
            finally
            {
                await Task.WhenAll(
                    passiveChannel.StopAsync(),
                    activeChannel.StopAsync());
            }
        }

        [TestMethod]
        public async Task Check_SimpleTcpIPConnection()
        {
            var testingPort = TestUtility.GetFreeTcpPort();

            var receiveTaskOnPassiveChannel = new TaskCompletionSource<string>();
            var receiveTaskOnActiveChannel = new TaskCompletionSource<string>();

            // Define channels
            var passiveChannel = new MessageChannel(
                new TcpPassiveByteStreamHandlerSettings(IPAddress.Loopback, testingPort),
                new DefaultMessageRecognizerSettings(Encoding.UTF8),
                (msg) =>
                {
                    receiveTaskOnPassiveChannel.SetResult(msg.ToString());
                    msg.ReturnToPool();
                });
            var activeChannel = new MessageChannel(
                new TcpActiveByteStreamHandlerSettings("127.0.0.1", testingPort),
                new DefaultMessageRecognizerSettings(Encoding.UTF8),
                (msg) =>
                {
                    receiveTaskOnActiveChannel.SetResult(msg.ToString());
                    msg.ReturnToPool();
                });
            try
            {
                // Start both channels
                await Task.WhenAll(
                    passiveChannel.StartAsync(),
                    activeChannel.StartAsync());

                // Wait for connection
                var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5.0));
                await Task.WhenAll(
                    passiveChannel.WaitForConnectionAsync(timeoutTokenSource.Token),
                    activeChannel.WaitForConnectionAsync(timeoutTokenSource.Token));
                Assert.IsTrue(passiveChannel.State == ConnectionState.Connected);
                Assert.IsTrue(activeChannel.State == ConnectionState.Connected);

                // Send/Receive some messages in both directions
                await passiveChannel.SendAsync("Message from passive endpoint");
                await activeChannel.SendAsync("Message from active endpoint");

                // Check for received messages
                var receivedOnPassiveChannel = await receiveTaskOnPassiveChannel.Task;
                var receivedOnActiveChannel = await receiveTaskOnActiveChannel.Task;
                Assert.IsTrue(receivedOnPassiveChannel == "Message from active endpoint");
                Assert.IsTrue(receivedOnActiveChannel == "Message from passive endpoint");
            }
            finally
            {
                await Task.WhenAll(
                    passiveChannel.StopAsync(),
                    activeChannel.StopAsync());
            }
        }
    }
}
