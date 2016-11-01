REM rmdir Output /Q /S nonemptydir

cd "../src/NSwag.AspNetCore"

del project.lock.json
dotnet restore --no-cache
dotnet pack --output "../../build/Packages" --configuration Release

cd "../NSwag.AssemblyLoaderCore"

del project.lock.json
dotnet restore --no-cache
dotnet pack --output "../../build/Packages" --configuration Release

cd "../NSwag.TerminalCore"

del project.lock.json
dotnet restore --no-cache
dotnet pack --output "../../build/Packages" --configuration Release

cd "../../build"