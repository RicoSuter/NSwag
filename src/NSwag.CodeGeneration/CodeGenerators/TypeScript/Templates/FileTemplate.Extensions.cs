using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.CodeGenerators.TypeScript.Models;

namespace NSwag.CodeGeneration.CodeGenerators.TypeScript.Templates
{
    internal partial class FileTemplate : ITemplate
    {
        public FileTemplate(object model)
        {
            Model = (FileTemplateModel)model;
        }

        public FileTemplateModel Model { get; }
        
        public string Render()
        {
            return ConversionUtilities.TrimWhiteSpaces(TransformText());
        }
    }
}
