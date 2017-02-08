using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.TypeScript.Models;

namespace NSwag.CodeGeneration.TypeScript.Templates
{
    internal partial class AngularJSClientTemplate : ITemplate
    {
        public AngularJSClientTemplate(TypeScriptClientTemplateModel model)
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
