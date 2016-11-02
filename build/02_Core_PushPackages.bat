set /p apiKey=NuGet API Key: 
set /p version=Package Version: 

nuget.exe push Packages/NSwag.AspNetCore.%version%.nupkg %apiKey% -s https://nuget.org
nuget.exe push Packages/NSwag.AssemblyLoaderCore.%version%.nupkg %apiKey% -s https://nuget.org
nuget.exe push Packages/NSwag.ConsoleCore.%version%.nupkg %apiKey% -s https://nuget.org