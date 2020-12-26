# ByteStreamHandler
The ByteStreamHandler is responsible for sending and receiving
binary packages. 


The pipeline is structured as follows:

### Sending:
[Local App] --> MessageRecognizer --> ByteStreamHandler --> [Channel]


### Receiving:
[Channel] --> ByteStreamHandler --> MessageRecognizer --> [LocalApp]