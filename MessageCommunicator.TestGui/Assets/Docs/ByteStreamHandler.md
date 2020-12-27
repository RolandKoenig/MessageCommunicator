# ByteStreamHandler
The ByteStreamHandler is responsible for sending and receiving
binary packages. For sending, it gets all bytes to be sent from the
MessageRecognizer. For receiving, it forwards all received bytes
to the MessageRecognizer.

MessageCommunicator supports the following types for ByteStreamHandler:
 - **Tcp**
   - *Target*: IP or hostname of the target host. This parameter is not relevant in passive mode.
   - *Port*: The port to listen or to connect to (depending on the mode).
   - *Mode*:
     - Active: Connect to a remote host and port in active mode.
     - Passive: Listen on a local port for incoming connections (Target parameter not relevant).
   - *Receive Timeout (Sec)*: When no packages are received during the given timespan, than a reconnect will be 
     triggered. 
 - **Udp**: 
   - *LocalPort*: The port on which to listen for incoming packages.
   - *RemoteHost*: The remote host (hostname or ip address) to which to send outgoing packages.
   - *RemotePort*: The remote port to which to send outgoing packages.
 - **Serial**: 
   - *Port*: 
    