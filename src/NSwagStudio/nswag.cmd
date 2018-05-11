@ECHO OFF
set args=%* 

IF NOT "%args:/runtime:winx86=%" == "%args%" (
    "%~dp0/Win/nswag.x86.exe" %*
    GOTO end
)

IF NOT "%args:/runtime:netcore10=%" == "%args%" (
    dotnet "%~dp0/NetCore10/dotnet-nswag.dll" %*
    GOTO end
)

IF NOT "%args:/runtime:netcore11=%" == "%args%" (
    dotnet "%~dp0/NetCore11/dotnet-nswag.dll" %*
    GOTO end
)

IF NOT "%args:/runtime:netcore20=%" == "%args%" (
    dotnet "%~dp0/NetCore20/dotnet-nswag.dll" %*
    GOTO end
)

IF NOT "%args:/runtime:netcore21=%" == "%args%" (
    dotnet "%~dp0/NetCore21/dotnet-nswag.dll" %*
    GOTO end
)

"%~dp0/Win/nswag.exe" %*
:end