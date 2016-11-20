using NJsonSchema;
using NJsonSchema.CodeGeneration;

namespace NSwag.CodeGeneration.CodeGenerators.CSharp.Templates
{
    internal partial class FileTemplate : ITemplate
    {
        public FileTemplate(object model)
        {
            Model = model;
        }

        public dynamic Model { get; }
        
        public string Render()
        {
            return ConversionUtilities.TrimWhiteSpaces(TransformText());
        }
    }
}
