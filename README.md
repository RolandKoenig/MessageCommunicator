# TcpCommunicator
### About
A small testing project for async tcp communication and Avalonia UI. Further, the tool is meant to be of help when it comes to testing tcp connections with other machines / software.

Some rules I follow by implementing the tcp/ip interface:
 - Start TcpCommunicator in passive (listening) or active mode
 - Only single point-to-point connections. If a passive TcpCommunicator receives a second connection request while it already has a connection, then it closes the old one and accepts the new one
 - Only Tcp mode
 - Send and receive of single line messages

I use the following technologies / projects:
 - [Avalonia](https://github.com/AvaloniaUI/Avalonia): Crossplatform, Xaml based UI framework
 - [Avalonia.IconPacks](https://github.com/ahopper/Avalonia.IconPacks): A good collection of free vector icons ready to be used in Avalonia applications
 - [ReactiveUI](https://github.com/reactiveui/ReactiveUI): Crossplatform mvvm framework. Avalonia has additional integration for ReactiveUI
 - [StringFormatter](https://github.com/MikePopoloski/StringFormatter): A copy/paste ready alternative to StringBuilder. StringFormatter is optimized for less object allocations

### Screenshot
![alt text](_Misc/Screenshot_01.png "Screenshot of the testing UI")