@ECHO OFF
set args=%*

IF NOT "%args:/runtime:winx86=%" == "%args%" (
    "%~dp0/Win/nswag.x86.exe" %*
    GOTO end
)

IF NOT "%args:/runtime:net80=%" == "%args%" (
    dotnet "%~dp0/Net80/dotnet-nswag.dll" %*
    GOTO end
)

IF NOT "%args:/runtime:net90=%" == "%args%" (
    dotnet "%~dp0/Net90/dotnet-nswag.dll" %*
    GOTO end
)

IF NOT "%args:/runtime:net100=%" == "%args%" (
    dotnet "%~dp0/Net100/dotnet-nswag.dll" %*
    GOTO end
)

"%~dp0/Win/nswag.exe" %*
:end
