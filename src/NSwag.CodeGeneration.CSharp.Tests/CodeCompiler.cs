using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace NSwag.CodeGeneration.CSharp.Tests;

public static class CodeCompiler
{
    private static readonly List<PortableExecutableReference> MetadataReferences;

    static CodeCompiler()
    {
        MetadataReferences = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location))
            .Select(x => MetadataReference.CreateFromFile(x.Location))
            .Append(MetadataReference.CreateFromFile(typeof(System.ComponentModel.DataAnnotations.RangeAttribute).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(System.Collections.ObjectModel.ObservableCollection<>).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(System.Runtime.Serialization.EnumMemberAttribute).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(System.Text.Json.Serialization.JsonConverter).Assembly.Location))
            .ToList();
    }

    public static void AssertCompile(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "assemblyName",
            syntaxTrees: [syntaxTree],
            references:
            MetadataReferences,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var dllStream = new MemoryStream();
        using var pdbStream = new MemoryStream();

        var emitResult = compilation.Emit(dllStream, pdbStream);
        if (!emitResult.Success)
        {
            // emitResult.Diagnostics
            Assert.Empty(emitResult.Diagnostics);
        }
    }
}