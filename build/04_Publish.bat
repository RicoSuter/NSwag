pushd "%~dp0\.."
cmd /c call build.cmd publish --configuration Release
popd
