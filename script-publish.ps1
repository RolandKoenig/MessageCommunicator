Remove-Item -Path ./publish/* -Recurse -Force

# Publish gui application
dotnet publish -c Release -f netcoreapp3.1 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true --runtime win-x86 -o ./publish/MessageCommunicator.TestGui ./MessageCommunicator.TestGui