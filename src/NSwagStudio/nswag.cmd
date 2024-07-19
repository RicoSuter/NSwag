@ECHO OFF
set args=%*

IF NOT "%args:/runtime:winx86=%" == "%args%" (
    "%~dp0/Win/nswag.x86.exe" %*
    GOTO end
)

IF NOT "%args:/runtime:net60=%" == "%args%" (
    dotnet "%~dp0/Net60/dotnet-nswag.dll" %*
    GOTO end
)

IF NOT "%args:/runtime:net80=%" == "%args%" (
    dotnet "%~dp0/Net80/dotnet-nswag.dll" %*
    GOTO end
)

"%~dp0/Win/nswag.exe" %*
:end
