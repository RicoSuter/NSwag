rmdir Output /Q /S nonemptydir

del NSwag.AssemblyLoaderCore/project.lock.json
dotnet restore NSwag.AssemblyLoaderCore/ --no-cache
dotnet pack --output Output --configuration Release