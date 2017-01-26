using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.CodeGenerators.Models;

namespace NSwag.CodeGeneration.CodeGenerators.TypeScript.Templates
{
    internal partial class RequestUrlTemplate : ITemplate
    {
        public RequestUrlTemplate(OperationModelBase model)
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
