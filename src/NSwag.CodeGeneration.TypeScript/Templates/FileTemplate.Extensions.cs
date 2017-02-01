using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.TypeScript.Models;

namespace NSwag.CodeGeneration.TypeScript.Templates
{
    internal partial class FileTemplate : ITemplate
    {
        public FileTemplate(object model)
        {
            Model = (TypeScriptFileTemplateModel)model;
        }

        public TypeScriptFileTemplateModel Model { get; }
        
        public string Render()
        {
            return ConversionUtilities.TrimWhiteSpaces(TransformText());
        }
    }
}
