rm -rf bin/Release
rm -rf ItemsDBEditor-*
dotnet publish -r win-x86 -c Release /p:PublishSingleFile=true
zip -r ./ItemsDBEditor-x86.zip bin/Release/netcoreapp3.1/win-x86/publish
dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true
zip -r ./ItemsDBEditor-x64.zip bin/Release/netcoreapp3.1/win-x64/publish
dotnet publish -r osx-x64 -c Release /p:PublishSingleFile=true
zip -r ./ItemsDBEditor-OSX.zip bin/Release/netcoreapp3.1/osx-x64/publish
dotnet publish -r linux-x64 -c Release /p:PublishSingleFile=true
zip -r ./ItemsDBEditor-Linux-x64.zip bin/Release/netcoreapp3.1/linux-x64/publish
