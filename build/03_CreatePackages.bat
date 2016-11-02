mkdir Packages
nuget pack ../src/NSwag.Core/NSwag.Core.csproj -IncludeReferencedProjects -OutputDirectory "Packages" -Prop Configuration=Release
nuget pack ../src/NSwag.Commands/NSwag.Commands.csproj -IncludeReferencedProjects -OutputDirectory "Packages" -Prop Configuration=Release
nuget pack ../src/NSwag.CodeGeneration/NSwag.CodeGeneration.csproj -IncludeReferencedProjects -OutputDirectory "Packages" -Prop Configuration=Release
nuget pack ../src/NSwag.Annotations/NSwag.Annotations.csproj -IncludeReferencedProjects -OutputDirectory "Packages" -Prop Configuration=Release
