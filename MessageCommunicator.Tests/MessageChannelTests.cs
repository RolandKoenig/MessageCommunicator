using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using MessageCommunicator.SerialPorts;
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

        [TestMethod]
        public async Task Check_SerialConnection_Send()
        {
            var encoding = Encoding.UTF8;

            // Fake serial port api and listen for Write calls
            var receivedString = string.Empty;
            SerialPortByteStreamHandler.SerialPortFactory = () =>
            {
                var isOpened = false;

                var fakedSerialPort = A.Fake<ISerialPort>();
                A.CallTo(() => fakedSerialPort.IsOpen).ReturnsLazily(call => isOpened);
                A.CallTo(() => fakedSerialPort.Open()).Invokes(call => isOpened = true);
                A.CallTo(() => fakedSerialPort.Close()).Invokes(call => isOpened = false);
                A.CallTo(fakedSerialPort)
                    .Where(call => call.Method.Name == nameof(ISerialPort.Write))
                    .Invokes(call =>
                    {
                        var buffer = (byte[]) call.Arguments[0]!;
                        var offset = (int) call.Arguments[1]!;
                        var count = (int) call.Arguments[2]!;

                        receivedString = encoding.GetString(buffer, offset, count);
                    });

                return fakedSerialPort;
            };

            // Define channels
            var serialChannel = new MessageChannel(
                new SerialPortByteStreamHandlerSettings("COM1"),
                new StartAndEndSymbolsRecognizerSettings(encoding, "<", ">"));
            try
            {
                // Start both channels
                await serialChannel.StartAsync();

                // Wait for connection
                var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5.0));
                await serialChannel.WaitForConnectionAsync(timeoutTokenSource.Token);
                Assert.AreEqual(serialChannel.State, ConnectionState.Connected);

                // Send/Receive some messages in both directions
                await serialChannel.SendAsync("Message sent through serial channel");

                // Check for written message
                Assert.AreEqual(receivedString, "<Message sent through serial channel>");
            }
            finally
            {
                await serialChannel.StopAsync();
            }
        }

        [TestMethod]
        public async Task Check_SerialConnection_Receive()
        {
            var encoding = Encoding.UTF8;

            // Fake serial port api and listen for Write calls
            byte[]? fakeReceive = null;
            ISerialPort? fakedSerialPort = null;
            SerialPortByteStreamHandler.SerialPortFactory = () =>
            {
                var isOpened = false;

                fakedSerialPort = A.Fake<ISerialPort>();
                A.CallTo(() => fakedSerialPort.IsOpen).ReturnsLazily(call => isOpened);
                A.CallTo(() => fakedSerialPort.Open()).Invokes(call => isOpened = true);
                A.CallTo(() => fakedSerialPort.Close()).Invokes(call => isOpened = false);
                A.CallTo(() => fakedSerialPort.BytesToRead).ReturnsLazily(call => fakeReceive?.Length ?? 0);
                A.CallTo(fakedSerialPort)
                    .Where(call => call.Method.Name == nameof(ISerialPort.Read))
                    .WithReturnType<int>()
                    .Invokes(call =>
                    {
                        var fakeReceiveInner = fakeReceive;
                        if (fakeReceiveInner == null) { return; }

                        var buffer = (byte[]) call.Arguments[0]!;
                        Array.Copy(fakeReceiveInner, buffer, fakeReceiveInner.Length);
                    })
                    .ReturnsLazily(call =>
                    {
                        var result = fakeReceive!.Length;
                        fakeReceive = null;
                        return result;
                    });

                return fakedSerialPort;
            };

            // Define channels
            var receiveMessageTask = new TaskCompletionSource<string>();
            var serialChannel = new MessageChannel(
                new SerialPortByteStreamHandlerSettings("COM1"),
                new StartAndEndSymbolsRecognizerSettings(encoding, "<", ">"),
                (msg) =>
                {
                    receiveMessageTask.SetResult(msg.ToString());
                    msg.ReturnToPool();
                });
            try
            {
                // Start both channels
                await serialChannel.StartAsync();

                // Wait for connection
                var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5.0));
                await serialChannel.WaitForConnectionAsync(timeoutTokenSource.Token);
                Assert.AreEqual(serialChannel.State, ConnectionState.Connected);

                var constructor = typeof(SerialDataReceivedEventArgs)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, 
                        null, CallingConventions.Any,
                        new[]{ typeof(SerialData)}, null);
                Assert.IsNotNull(constructor, $"Constructor for {nameof(SerialDataReceivedEventArgs)} not found!");

                // Trigger receive data and event
                fakeReceive = encoding.GetBytes("<Message received through serial channel>");
                fakedSerialPort!.DataReceived += Raise.FreeForm<SerialDataReceivedEventHandler>
                    .With(
                        fakedSerialPort, 
                        constructor!.Invoke(new object[]{ SerialData.Chars }));

                // Process received data
                await Task.WhenAny(
                    receiveMessageTask.Task,
                    Task.Delay(4000));
                Assert.IsTrue(receiveMessageTask.Task.IsCompleted);
                Assert.AreEqual(receiveMessageTask.Task.Result, "Message received through serial channel");
            }
            finally
            {
                await serialChannel.StopAsync();
            }

            Assert.IsFalse(fakedSerialPort.IsOpen);
        }
    }
}
