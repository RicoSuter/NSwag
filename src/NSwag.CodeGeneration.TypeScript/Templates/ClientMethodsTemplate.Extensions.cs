using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.TypeScript.Models;

namespace NSwag.CodeGeneration.TypeScript.Templates
{
    internal partial class ClientMethodsTemplate : ITemplate
    {
        public ClientMethodsTemplate(TypeScriptFileTemplateModel model)
        {
            Model = model;
        }

        public TypeScriptFileTemplateModel Model { get; }

        public string Render()
        {
            return ConversionUtilities.TrimWhiteSpaces(TransformText());
        }
    }
}
