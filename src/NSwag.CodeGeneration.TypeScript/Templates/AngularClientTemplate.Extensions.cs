using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.TypeScript.Models;

namespace NSwag.CodeGeneration.TypeScript.Templates
{
    internal partial class AngularClientTemplate : ITemplate
    {
        public AngularClientTemplate(TypeScriptClientTemplateModel model)
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
