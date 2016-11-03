cd ../src/NSwag.ConsoleCore

dotnet restore --no-cache
dotnet build

dotnet publish -c release

rmdir "..\NSwag.ConsoleCore.Npm\bin\binaries" /Q /S nonemptydir
mkdir "..\NSwag.ConsoleCore.Npm\bin\binaries"
xcopy "bin\release\netcoreapp1.0\publish" "..\NSwag.ConsoleCore.Npm\bin\binaries" /E /I /y

cd ../../build