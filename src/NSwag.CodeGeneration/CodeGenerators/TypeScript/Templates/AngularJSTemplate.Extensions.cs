using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.CodeGenerators.TypeScript.Models;

namespace NSwag.CodeGeneration.CodeGenerators.TypeScript.Templates
{
    internal partial class AngularJSTemplate : ITemplate
    {
        public ClientTemplateModel Model { get; set; }

        /// <summary>Initializes the template with a model.</summary>
        /// <param name="model">The model.</param>
        public void Initialize(object model)
        {
            Model = (ClientTemplateModel)model;
        }

        /// <summary>Renders the template.</summary>
        /// <returns>The output.</returns>
        public string Render()
        {
            return ConversionUtilities.TrimWhiteSpaces(TransformText());
        }
    }
}
