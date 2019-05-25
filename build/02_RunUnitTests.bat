vstest.console /logger:Appveyor "%~dp0../src/NSwag.Generation.WebApi.Tests/bin/Release/NSwag.Generation.WebApi.Tests.dll" || goto :error
REM vstest.console /logger:Appveyor "%~dp0../src/NSwag.Tests/bin/Release/NSwag.Tests.dll" || goto :error

dotnet test "%~dp0/../src/NSwag.CodeGeneration.Tests/NSwag.CodeGeneration.Tests.csproj" -c Release || goto :error
dotnet test "%~dp0/../src/NSwag.CodeGeneration.CSharp.Tests/NSwag.CodeGeneration.CSharp.Tests.csproj" -c Release || goto :error
dotnet test "%~dp0/../src/NSwag.CodeGeneration.TypeScript.Tests/NSwag.CodeGeneration.TypeScript.Tests.csproj" -c Release || goto :error
dotnet test "%~dp0/../src/NSwag.Generation.AspNetCore.Tests/NSwag.Generation.AspNetCore.Tests.csproj" -c Release || goto :error
dotnet test "%~dp0/../src/NSwag.Core.Tests/NSwag.Core.Tests.csproj" -c Release || goto :error
dotnet test "%~dp0/../src/NSwag.Core.Yaml.Tests/NSwag.Core.Yaml.Tests.csproj" -c Release || goto :error
dotnet test "%~dp0/../src/NSwag.AssemblyLoader.Tests/NSwag.AssemblyLoader.Tests.csproj" -c Release -f netcoreapp2.1 || goto :error

goto :EOF
:error
echo Failed with error #%errorlevel%.
exit /b %errorlevel%
