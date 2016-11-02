rmdir Packages /Q /S nonemptydir
mkdir Packages

cd "../src/NSwag.AspNetCore"

del project.lock.json
dotnet restore --no-cache
dotnet pack --output "../../build/Packages" --configuration Release

cd "../NSwag.AssemblyLoaderCore"

del project.lock.json
dotnet restore --no-cache
dotnet pack --output "../../build/Packages" --configuration Release

cd "../NSwag.ConsoleCore"

del project.lock.json
dotnet restore --no-cache
dotnet pack --output "../../build/Packages" --configuration Release

cd "../../build"