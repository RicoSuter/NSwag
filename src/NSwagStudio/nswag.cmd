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

IF NOT "%args:/runtime:netcore22=%" == "%args%" (
    dotnet "%~dp0/NetCore22/dotnet-nswag.dll" %*
    GOTO end
)

IF NOT "%args:/runtime:netcore30=%" == "%args%" (
    dotnet "%~dp0/NetCore30/dotnet-nswag.dll" %*
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

"%~dp0/Win/nswag.exe" %*
:end