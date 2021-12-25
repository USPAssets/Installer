# run me.

chcp 65001
cd .\SpaghettiCh2\
dotnet restore -r osx-x64
dotnet msbuild -t:BundleApp -p:RuntimeIdentifier=osx-x64 -p:Configuration=Release -p:UseAppHost=true -p:PublishSingleFile=False -p:PublishTrimmed=False -p:SelfContained=true -p:PublishDir=bin/Release/net5.0/publish/USPMac
cd ..
