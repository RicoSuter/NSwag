rmdir "%~dp0\..\src\NSwag.Npm\bin\binaries" /Q /S nonemptydir
mkdir "%~dp0\..\src\NSwag.Npm\bin\binaries"

REM Build and copy full .NET command line
"%~dp0/nuget.exe" restore "%~dp0/../src/NSwag.sln"
dotnet restore "%~dp0/../src/NSwag.sln"
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild" "%~dp0/../src/NSwag.sln" /p:Configuration=Release || goto :error

xcopy "%~dp0/../src/NSwag.Console/bin/Release/net461" "%~dp0/../src/NSwag.Npm/bin/binaries/Win" /E /I /y
xcopy "%~dp0\..\src\NSwag.Console.x86\bin\Release\net461\NSwag.x86.exe" "%~dp0\..\src\NSwag.Npm\bin\binaries\Win"
xcopy "%~dp0\..\src\NSwag.Console.x86\bin\Release\net461\NSwag.x86.exe.config" "%~dp0\..\src\NSwag.Npm\bin\binaries\Win"

REM Build and publish .NET Core command line done in prebuild event for NSwagStudio.Installer.wixproj
xcopy "%~dp0/../src/NSwag.ConsoleCore/bin/release/netcoreapp2.1/publish" "%~dp0/../src/NSwag.Npm/bin/binaries/NetCore21" /E /I /y
xcopy "%~dp0/../src/NSwag.ConsoleCore/bin/release/netcoreapp2.2/publish" "%~dp0/../src/NSwag.Npm/bin/binaries/NetCore22" /E /I /y
xcopy "%~dp0/../src/NSwag.ConsoleCore/bin/release/netcoreapp3.0/publish" "%~dp0/../src/NSwag.Npm/bin/binaries/NetCore30" /E /I /y
xcopy "%~dp0/../src/NSwag.ConsoleCore/bin/release/netcoreapp3.1/publish" "%~dp0/../src/NSwag.Npm/bin/binaries/NetCore31" /E /I /y
xcopy "%~dp0/../src/NSwag.ConsoleCore/bin/release/net5.0/publish" "%~dp0/../src/NSwag.Npm/bin/binaries/Net50" /E /I /y
xcopy "%~dp0/../src/NSwag.ConsoleCore/bin/release/ne6.0/publish" "%~dp0/../src/NSwag.Npm/bin/binaries/Net60" /E /I /y

REM Package nuspecs
"%~dp0/nuget.exe" pack "%~dp0/../src/NSwag.MSBuild/NSwag.MSBuild.nuspec" || goto :error
"%~dp0/nuget.exe" pack "%~dp0/../src/NSwag.ApiDescription.Client/NSwag.ApiDescription.Client.nuspec" || goto :error
"%~dp0/nuget.exe" pack "%~dp0/../src/NSwagStudio.Chocolatey/NSwagStudio.nuspec" || goto :error

goto :EOF
:error
echo Failed with error #%errorlevel%.
exit /b %errorlevel%
