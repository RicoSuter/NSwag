using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.CodeGenerators.Models;
using NSwag.CodeGeneration.CodeGenerators.TypeScript.Templates;

namespace NSwag.CodeGeneration.CodeGenerators.TypeScript
{
    /// <summary>It generates request body using <see cref="RequestBodyTemplate"/></summary>
    public class RequestBodyGenerator
    {
        /// <summary>Renders the reqeust body.</summary>
        /// <param name="model">The model.</param>
        /// <param name="tabCount">The tab count.</param>
        /// <returns>Rendered request body</returns>
        public static string Render(ParameterModel model, int tabCount = 0)
        {
            var tpl = new RequestBodyTemplate(model);
            return ConversionUtilities.Tab(tpl.Render(), tabCount);
        }
    }
}