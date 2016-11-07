rmdir "..\src\NSwag.Npm\bin\binaries" /Q /S nonemptydir
mkdir "..\src\NSwag.Npm\bin\binaries"

REM Build and copy full .NET command line
nuget restore ../src/NSwag.sln
msbuild ../src/NSwag.sln /p:Configuration=Release /t:rebuild

xcopy "../src/NSwag.Console/bin/Release" "../src/NSwag.Npm/bin/binaries/full" /E /I /y

REM Build and copy .NET Core command line

dotnet restore "../src/NSwag.ConsoleCore" --no-cache
dotnet build "../src/NSwag.ConsoleCore"
dotnet publish "../src/NSwag.ConsoleCore" -c release

xcopy "../src/NSwag.ConsoleCore/bin/release/netcoreapp1.0/publish" "../src/NSwag.Npm/bin/binaries/core" /E /I /y