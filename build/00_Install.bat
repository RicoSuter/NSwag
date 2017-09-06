pushd "%~dp0\..\src\NSwag.Npm"
cmd /c call npm install
popd

pushd "%~dp0\..\src\NSwag.Integration.TypeScriptWeb"
cmd /c call npm install
popd