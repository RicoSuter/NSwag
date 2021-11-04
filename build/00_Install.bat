pushd "%~dp0\.."
cmd /c call build.cmd restore --configuration Release
popd
