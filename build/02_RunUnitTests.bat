vstest.console /logger:Appveyor "%~dp0../src/NSwag.CodeGeneration.CSharp.Tests/bin/Release/NSwag.CodeGeneration.CSharp.Tests.dll" || goto :error
vstest.console /logger:Appveyor "%~dp0../src\NSwag.CodeGeneration.Tests\bin/Release\NSwag.CodeGeneration.Tests.dll" || goto :error
vstest.console /logger:Appveyor "%~dp0../src\NSwag.CodeGeneration.TypeScript.Tests\bin/Release\NSwag.CodeGeneration.TypeScript.Tests.dll" || goto :error
vstest.console /logger:Appveyor "%~dp0../src\NSwag.SwaggerGeneration.WebApi.Tests\bin/Release/NSwag.SwaggerGeneration.WebApi.Tests.dll" || goto :error
vstest.console /logger:Appveyor "%~dp0../src\NSwag.Tests\bin/Release/NSwag.Tests.dll" || goto :error

dotnet test "%~dp0/../src/NSwag.SwaggerGeneration.AspNetCore.Tests/NSwag.SwaggerGeneration.AspNetCore.Tests.csproj" -c Release || goto :error
dotnet test "%~dp0/../src/NSwag.Core.Tests/NSwag.Core.Tests.csproj" -c Release || goto :error

goto :EOF
:error
echo Failed with error #%errorlevel%.
exit /b %errorlevel%
