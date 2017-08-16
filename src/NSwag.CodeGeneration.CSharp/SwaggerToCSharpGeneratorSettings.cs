//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;

namespace NSwag.CodeGeneration.CSharp
{
    /// <summary>Settings for the <see cref="SwaggerToCSharpGeneratorBase"/>.</summary>
    public abstract class SwaggerToCSharpGeneratorSettings : ClientGeneratorBaseSettings
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerToCSharpClientGeneratorSettings"/> class.</summary>
        protected SwaggerToCSharpGeneratorSettings()
        {
            AdditionalNamespaceUsages = null;
            CSharpGeneratorSettings = new CSharpGeneratorSettings
            {
                Namespace = "MyNamespace",
                NullHandling = NullHandling.Swagger, 
                TemplateFactory = new DefaultTemplateFactory()
            };

            GenerateResponseClasses = true;
            ResponseClass = "SwaggerResponse";
        }

        /// <summary>Gets or sets the CSharp generator settings.</summary>
        public CSharpGeneratorSettings CSharpGeneratorSettings { get; }

        /// <summary>Gets or sets the additional namespace usages.</summary>
        public string[] AdditionalNamespaceUsages { get; set; }

        /// <summary>Gets or sets the additional contract namespace usages.</summary>
        public string[] AdditionalContractNamespaceUsages { get; set; }

        /// <summary>Gets the code generator settings.</summary>
        public override CodeGeneratorSettingsBase CodeGeneratorSettings => CSharpGeneratorSettings;

        /// <summary>Gets or sets a value indicating whether to wrap success responses to allow full response access (experimental).</summary>
        public bool WrapResponses { get; set; }

        /// <summary>Gets or sets a value indicating whether to generate the response classes (only needed when WrapResponses == true, default: true).</summary>
        public bool GenerateResponseClasses { get; set; }

        /// <summary>Gets or sets the name of the response class (supports the '{controller}' placeholder).</summary>
        public string ResponseClass { get; set; }
    }
}