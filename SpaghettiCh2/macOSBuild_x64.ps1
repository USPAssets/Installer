# run me.

chcp 65001
cd .\SpaghettiCh2\
dotnet restore -r osx-x64
dotnet msbuild -t:BundleApp -p:RuntimeIdentifier=osx-x64 -p:Configuration=Release -p:UseAppHost=true -p:PublishSingleFile=False -p:PublishTrimmed=False -p:SelfContained=false -p:PublishDir=bin/Release/net8.0/publish/osx-x64/
cd ..
