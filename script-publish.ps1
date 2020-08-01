Remove-Item -Path ./publish/* -Recurse -Force

# Publish gui application
dotnet publish -c Release -f netcoreapp3.1 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true --runtime win-x86 -o "./publish/MessageCommunicator (Win X86)" ./MessageCommunicator.TestGui
dotnet publish -c Release -f netcoreapp3.1 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true --runtime win-x64 -o "./publish/MessageCommunicator (Win X64)" ./MessageCommunicator.TestGui
dotnet publish -c Release -f netcoreapp3.1 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true --runtime linux-x64 -o "./publish/MessageCommunicator (Linux X64)" ./MessageCommunicator.TestGui