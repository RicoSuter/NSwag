using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration.CodeGenerators.CSharp.Models;

namespace NSwag.CodeGeneration.CodeGenerators.CSharp.Templates
{
    internal partial class ClientTemplate : ITemplate
    {
        public ClientModel Model { get; set; }

        /// <summary>Initializes the template with a model.</summary>
        /// <param name="model">The model.</param>
        public void Initialize(object model)
        {
            Model = (ClientModel)model;
        }

        /// <summary>Renders the template.</summary>
        /// <returns>The output.</returns>
        public string Render()
        {
            return ConversionUtilities.TrimWhiteSpaces(TransformText());
        }
    }
}
