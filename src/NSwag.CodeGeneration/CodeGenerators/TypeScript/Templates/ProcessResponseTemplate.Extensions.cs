using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.CodeGenerators.Models;

namespace NSwag.CodeGeneration.CodeGenerators.TypeScript.Templates
{
    internal partial class ProcessResponseTemplate : ITemplate
    {
        public ProcessResponseTemplate(OperationModelBase model)
        {
            Model = model;
        }

        public OperationModelBase Model { get; }

        public string Render()
        {
            return ConversionUtilities.TrimWhiteSpaces(TransformText());
        }
    }
}
