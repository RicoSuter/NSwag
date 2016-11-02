nuget restore ../src/NSwag.sln
msbuild ../src/NSwag.sln /p:Configuration=Release /t:rebuild
