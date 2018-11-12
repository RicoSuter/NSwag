vstest.console /logger:Appveyor "%~dp0../src/NSwag.SwaggerGeneration.WebApi.Tests/bin/Release/NSwag.SwaggerGeneration.WebApi.Tests.dll" || goto :error
vstest.console /logger:Appveyor "%~dp0../src/NSwag.Tests/bin/Release/NSwag.Tests.dll" || goto :error

dotnet test "%~dp0/../src/NSwag.CodeGeneration.Tests/NSwag.CodeGeneration.Tests.csproj" -c Release || goto :error
dotnet test "%~dp0/../src/NSwag.CodeGeneration.CSharp.Tests/NSwag.CodeGeneration.CSharp.Tests.csproj" -c Release || goto :error
dotnet test "%~dp0/../src/NSwag.CodeGeneration.TypeScript.Tests/NSwag.CodeGeneration.TypeScript.Tests.csproj" -c Release || goto :error
dotnet test "%~dp0/../src/NSwag.SwaggerGeneration.AspNetCore.Tests/NSwag.SwaggerGeneration.AspNetCore.Tests.csproj" -c Release || goto :error
dotnet test "%~dp0/../src/NSwag.Core.Tests/NSwag.Core.Tests.csproj" -c Release || goto :error
dotnet test "%~dp0/../src/NSwag.Core.Yaml.Tests/NSwag.Core.Yaml.Tests.csproj" -c Release || goto :error
dotnet test "%~dp0/../src/NSwag.AssemblyLoader.Tests/NSwag.AssemblyLoader.Tests.csproj" -c Release -f netcoreapp2.1 || goto :error

goto :EOF
:error
echo Failed with error #%errorlevel%.
exit /b %errorlevel%
