using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.CSharp.Models;

namespace NSwag.CodeGeneration.CSharp.Templates
{
    internal partial class ClientTemplate : ITemplate
    {
        public ClientTemplate(CSharpClientTemplateModel model)
        {
            Model = model;
        }

        public CSharpClientTemplateModel Model { get; }

        public string Render()
        {
            return ConversionUtilities.TrimWhiteSpaces(TransformText());
        }
    }
}
