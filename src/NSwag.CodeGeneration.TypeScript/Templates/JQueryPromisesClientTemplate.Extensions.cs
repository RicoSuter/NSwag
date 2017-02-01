using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.TypeScript.Models;

namespace NSwag.CodeGeneration.TypeScript.Templates
{
    internal partial class JQueryPromisesClientTemplate : ITemplate
    {
        public JQueryPromisesClientTemplate(TypeScriptClientTemplateModel model)
        {
            Model = model;
        }

        public TypeScriptClientTemplateModel Model { get; }
        
        public string Render()
        {
            return ConversionUtilities.TrimWhiteSpaces(TransformText());
        }
    }
}
