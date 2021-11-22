pushd "%~dp0\.."
cmd /c call build.cmd unittest --configuration Release
popd
