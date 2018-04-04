REM pushd "%~dp0\..\src\NSwag.Sample.NetCoreAngular"
REM dotnet publish || goto :error
REM "%~dp0\..src\NSwagStudio\bin\Release\nswag" run /runtime:NetCore11 || goto :error
REM popd

pushd "%~dp0\..\src\NSwag.Sample.NETCore11"
dotnet publish || goto :error
cmd /c call "%~dp0\..\src\NSwagStudio\bin\Release\nswag.cmd" run /runtime:NetCore11 || goto :error
popd

pushd "%~dp0\..\src\NSwag.Sample.NETCore20"
dotnet publish || goto :error
cmd /c call "%~dp0\..\src\NSwagStudio\bin\Release\nswag.cmd" run /runtime:NetCore20 || goto :error
popd

pushd "%~dp0\..\src\NSwag.Sample.NetGlobalAsax"
msbuild || goto :error
cmd /c call "%~dp0\..\src\NSwagStudio\bin\Release\nswag.cmd" run /runtime:Winx64 || goto :error
popd

pushd "%~dp0\..\src\NSwag.Integration.WebAPI"
msbuild || goto :error
cmd /c call "%~dp0\..\src\NSwagStudio\bin\Release\nswag.cmd" run /runtime:Winx64 || goto :error
popd

goto :EOF
:error
echo Failed with error #%errorlevel%.
exit /b %errorlevel%
