pushd "%~dp0\.."
cmd /c call build.cmd compile pack --configuration Release
popd
