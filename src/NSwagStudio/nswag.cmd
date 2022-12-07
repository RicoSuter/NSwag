@ECHO OFF
set args=%*

IF NOT "%args:/runtime:winx86=%" == "%args%" (
    "%~dp0/Win/nswag.x86.exe" %*
    GOTO end
)

IF NOT "%args:/runtime:netcore21=%" == "%args%" (
    dotnet "%~dp0/NetCore21/dotnet-nswag.dll" %*
    GOTO end
)

IF NOT "%args:/runtime:netcore31=%" == "%args%" (
    dotnet "%~dp0/NetCore31/dotnet-nswag.dll" %*
    GOTO end
)

IF NOT "%args:/runtime:net50=%" == "%args%" (
    dotnet "%~dp0/Net50/dotnet-nswag.dll" %*
    GOTO end
)

IF NOT "%args:/runtime:net60=%" == "%args%" (
    dotnet "%~dp0/Net60/dotnet-nswag.dll" %*
    GOTO end
)

IF NOT "%args:/runtime:net70=%" == "%args%" (
    dotnet "%~dp0/Net70/dotnet-nswag.dll" %*
    GOTO end
)

"%~dp0/Win/nswag.exe" %*
:end
