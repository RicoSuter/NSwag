using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.CodeGenerators.TypeScript.Models;

namespace NSwag.CodeGeneration.CodeGenerators.TypeScript.Templates
{
    internal partial class JQueryPromisesClientTemplate : ITemplate
    {
        public JQueryPromisesClientTemplate(ClientTemplateModel model)
        {
            Model = model;
        }

        public ClientTemplateModel Model { get; }
        
        public string Render()
        {
            return ConversionUtilities.TrimWhiteSpaces(TransformText());
        }
    }
}
