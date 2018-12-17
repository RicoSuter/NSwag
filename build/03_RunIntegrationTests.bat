REM pushd "%~dp0\..\src\NSwag.Sample.NetCoreAngular"
REM dotnet publish || goto :error
REM "%~dp0\..src\NSwagStudio\bin\Release\nswag" run /runtime:NetCore11 || goto :error
REM popd

pushd "%~dp0\..\samples"
cmd /c call powershell .\run.ps1 Release || goto :error
popd

pushd "%~dp0\..\src\NSwag.Sample.NETCore11"
dotnet restore || goto :error
dotnet publish || goto :error
cmd /c call "..\NSwagStudio\bin\Release\nswag.cmd" run /runtime:NetCore11 || goto :error
popd

pushd "%~dp0\..\src\NSwag.Sample.NETCore20"
dotnet restore || goto :error
dotnet publish || goto :error
cmd /c call "..\NSwagStudio\bin\Release\nswag.cmd" run /runtime:NetCore20 || goto :error
popd

pushd "%~dp0\..\src\NSwag.Sample.NETCore21"
dotnet restore || goto :error
dotnet build /p:CopyLocalLockFileAssemblies=true || goto :error
cmd /c call "..\NSwagStudio\bin\Release\nswag.cmd" run /runtime:NetCore21 || goto :error
popd

pushd "%~dp0\..\src\NSwag.Sample.NETCore22"
dotnet restore || goto :error
dotnet build /p:CopyLocalLockFileAssemblies=true || goto :error
cmd /c call "..\NSwagStudio\bin\Release\nswag.cmd" run /runtime:NetCore22 || goto :error
popd

pushd "%~dp0\..\src\NSwag.Sample.NetGlobalAsax"
msbuild || goto :error
cmd /c call "..\NSwagStudio\bin\Release\nswag.cmd" run /runtime:Winx64 || goto :error
popd

pushd "%~dp0\..\src\NSwag.Integration.WebAPI"
msbuild || goto :error
cmd /c call "..\NSwagStudio\bin\Release\nswag.cmd" run /runtime:Winx64 || goto :error
popd

goto :EOF
:error
echo Failed with error #%errorlevel%.
exit /b %errorlevel%
