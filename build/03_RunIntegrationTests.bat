pushd "%~dp0\.."
cmd /c call build.cmd integrationtest --configuration Release
popd
