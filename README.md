# MessageCommunicator
## About
MessageCommunicator is a library + testing Gui for message-based communication. 
The library is based on .Net Standard 2.1. The Gui is build using C# and Avalonia, 
therefore it supports the desktop environments on Windows, Linux and Mac.

### Library
The library is designed to be cross-platform, asynchronous and to use as less object allocations as possible.
In the following example we are creating a channel which listens for incoming tcp connections on
port 12000. Messages are encoded by UTF8 and use ## as end sign.

```csharp
// Create and start passive channel (listens for incoming tcp connection)
var passiveTcpChannel = new MessageChannel(
    new TcpPassiveByteStreamHandlerSettings(IPAddress.Loopback, 12000),
    new EndSymbolsMessageRecognizerSettings(Encoding.UTF8, "##"),
    (message) =>
    {
        Console.WriteLine($"Received message on passive channel: {message}");
    });
await passiveTcpChannel.StartAsync();
```

In the following example we are doing almost the same. The only difference is that we do not 
listen. Here we are connecting to port 12000 on the localhost.

```csharp
// Create and start send channel
var activeTcpChannel = new MessageChannel(
    new TcpActiveByteStreamHandlerSettings("127.0.0.1", 12000), 
    new EndSymbolsMessageRecognizerSettings(Encoding.UTF8, "##"),
    (message) =>
    {
        Console.WriteLine($"Received message on active channel: {message}");
    });
await activeTcpChannel.StartAsync();
```

Sending a message is as easy as:
```csharp
await activeTcpChannel.SendAsync(new Message("Message 1 from active to passive..."));
```

**Nuget**: tbd.

### Gui

![alt text](_Misc/Screenshot_01.png "Screenshot of the testing UI")

## Based on...
MessageCommunicator is based on .Net Core and meant to be cross-platform. 

I use the following technologies / projects:
 - [Avalonia](https://github.com/AvaloniaUI/Avalonia): Crossplatform, Xaml based UI framework
 - [Avalonia.IconPacks](https://github.com/ahopper/Avalonia.IconPacks): A good collection of free vector icons ready to be used in Avalonia applications
 - [ReactiveUI](https://github.com/reactiveui/ReactiveUI): Crossplatform mvvm framework. Avalonia has additional integration for ReactiveUI
 - [StringFormatter](https://github.com/MikePopoloski/StringFormatter): A copy/paste ready alternative to StringBuilder. StringFormatter is optimized for less object allocations