rm -rf bin/Release
rm -rf ItemsDBEditor-*

dotnet publish -r win-x86 -c Release /p:PublishSingleFile=true
mkdir ItemsDBEditor-x86
mv bin/Release/netcoreapp3.1/win-x86/publish/* ./ItemsDBEditor-x86
zip -r ./ItemsDBEditor-x86.zip ItemsDBEditor-x86

dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true
mkdir ItemsDBEditor-x64
mv bin/Release/netcoreapp3.1/win-x64/publish/* ./ItemsDBEditor-x64
zip -r ./ItemsDBEditor-x64.zip ItemsDBEditor-x64

dotnet publish -r osx-x64 -c Release /p:PublishSingleFile=true
mkdir ItemsDBEditor-OSX
mv bin/Release/netcoreapp3.1/osx-x64/publish/* ./ItemsDBEditor-OSX
zip -r ./ItemsDBEditor-OSX.zip ItemsDBEditor-OSX

dotnet publish -r linux-x64 -c Release /p:PublishSingleFile=true
mkdir ItemsDBEditor-Linux-x64
mv bin/Release/netcoreapp3.1/linux-x64/publish/* ./ItemsDBEditor-Linux-x64
zip -r ./ItemsDBEditor-Linux-x64.zip ItemsDBEditor-Linux-x64