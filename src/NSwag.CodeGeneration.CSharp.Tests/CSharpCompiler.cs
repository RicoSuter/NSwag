using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace NSwag.CodeGeneration.CSharp.Tests;

public static class CSharpCompiler
{
    private static readonly List<PortableExecutableReference> MetadataReferences;

    static CSharpCompiler()
    {
        MetadataReferences = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location))
            .Select(x => MetadataReference.CreateFromFile(x.Location))
            .Append(MetadataReference.CreateFromFile(typeof(HttpClient).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(Microsoft.AspNetCore.Mvc.FileResult).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(Microsoft.AspNetCore.Http.IFormFile).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(Microsoft.AspNetCore.Http.HttpRequest).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(Microsoft.Extensions.Primitives.StringValues).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(Newtonsoft.Json.JsonSerializer).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(System.Net.HttpStatusCode).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(System.ComponentModel.DataAnnotations.RangeAttribute).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(System.Collections.ObjectModel.ObservableCollection<>).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(System.Runtime.Serialization.EnumMemberAttribute).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(System.Text.Json.Serialization.JsonConverter).Assembly.Location))
            // stubs
            .Append(MetadataReference.CreateFromFile(typeof(System.Web.Http.ApiController).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(System.Web.Http.FromBodyAttribute).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(System.Web.Http.FromUriAttribute).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(System.Web.Http.HttpGetAttribute).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(System.Web.Http.HttpPostAttribute).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(System.Web.Http.ParameterBindingAttribute).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(System.Web.Http.RouteAttribute).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(System.Web.Http.Controllers.HttpActionContext).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(System.Web.Http.Controllers.HttpParameterBinding).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(System.Web.Http.Controllers.HttpParameterDescriptor).Assembly.Location))
            .Append(MetadataReference.CreateFromFile(typeof(System.Web.Http.Metadata.ModelMetadataProvider).Assembly.Location))
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