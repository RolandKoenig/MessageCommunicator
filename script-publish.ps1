Remove-Item -Path ./publish/* -Recurse -Force

# Create nuget package for MessageCommunicator
dotnet pack -c Release -o ./publish ./MessageCommunicator

# Publish gui application
dotnet publish -c Release -f netcoreapp3.1 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true --runtime win-x86 -o "./publish/MessageCommunicator (Win X86)" ./MessageCommunicator.TestGui
dotnet publish -c Release -f netcoreapp3.1 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true --runtime win-x64 -o "./publish/MessageCommunicator (Win X64)" ./MessageCommunicator.TestGui
dotnet publish -c Release -f netcoreapp3.1 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true --runtime linux-x64 -o "./publish/MessageCommunicator (Linux X64)" ./MessageCommunicator.TestGui
dotnet publish -c Release -f netcoreapp3.1 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true --runtime osx-x64 -o "./publish/MessageCommunicator (macOS 10.12 X64)" ./MessageCommunicator.TestGui

# Remove pdb files
Copy-Item "./publish/MessageCommunicator (Win X86)/MessageCommunicator.TestGui.exe" -Destination "./publish/MessageCommunicator-Win-X86.exe"
Copy-Item "./publish/MessageCommunicator (Win X64)/MessageCommunicator.TestGui.exe" -Destination "./publish/MessageCommunicator-Win-X64.exe"
Copy-Item "./publish/MessageCommunicator (Linux X64)/MessageCommunicator.TestGui" -Destination "./publish/MessageCommunicator-Linux-X64"
Copy-Item "./publish/MessageCommunicator (macOS 10.12 X64)/MessageCommunicator.TestGui" -Destination "./publish/MessageCommunicator-macOS-X64"

# Compress each build for lower disk usage
$compress = @{
  Path = "./publish/MessageCommunicator-Win-X86.exe"
  CompressionLevel = "Optimal"
  DestinationPath = "./publish/MessageCommunicator-Win-X86.zip"
}
Compress-Archive @compress

$compress = @{
  Path = "./publish/MessageCommunicator-Win-X64.exe"
  CompressionLevel = "Optimal"
  DestinationPath = "./publish/MessageCommunicator-Win-X64.zip"
}
Compress-Archive @compress

$compress = @{
  Path = "./publish/MessageCommunicator-Linux-X64"
  CompressionLevel = "Optimal"
  DestinationPath = "./publish/MessageCommunicator-Linux-X64.zip"
}
Compress-Archive @compress

$compress = @{
  Path = "./publish/MessageCommunicator-macOS-X64"
  CompressionLevel = "Optimal"
  DestinationPath = "./publish/MessageCommunicator-macOS-X64.zip"
}
Compress-Archive @compress