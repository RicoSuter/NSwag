using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.CodeGenerators.Models;
using NSwag.CodeGeneration.CodeGenerators.TypeScript.Templates;

namespace NSwag.CodeGeneration.CodeGenerators.TypeScript
{
    internal class RequestBodyGenerator
    {
        internal static string Render(ParameterModel model, int tabCount = 0)
        {
            var tpl = new RequestBodyTemplate();
            tpl.Initialize(model);
            return ConversionUtilities.Tab(tpl.Render(), tabCount);
        }
    }
}