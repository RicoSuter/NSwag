using NJsonSchema;
using NJsonSchema.CodeGeneration;

namespace NSwag.CodeGeneration.CodeGenerators.CSharp.Templates
{
    internal partial class ControllerTemplate : ITemplate
    {
        public ControllerTemplate(object model)
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
