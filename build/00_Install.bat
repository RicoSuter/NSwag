pushd "%~dp0\.."
cmd /c call build.cmd install --configuration Release
popd
