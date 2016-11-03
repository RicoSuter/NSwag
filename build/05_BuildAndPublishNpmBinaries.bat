cd ../src/NSwag.ConsoleCore

dotnet restore --no-cache
dotnet build

dotnet publish -c release

xcopy "bin\release\netcoreapp1.0\publish" "..\NSwag.ConsoleCore.Npm\bin\binaries"

cd ../../build