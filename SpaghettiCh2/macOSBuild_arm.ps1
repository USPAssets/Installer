# run me.

chcp 65001
cd .\SpaghettiCh2\
dotnet restore -r osx-arm64
dotnet msbuild -t:BundleApp -p:RuntimeIdentifier=osx-arm64 -p:Configuration=Release -p:UseAppHost=true -p:PublishSingleFile=false -p:PublishTrimmed=false -p:SelfContained=false -p:PublishDir=bin/Release/net8.0/publish/osx-arm64/
cd ..
