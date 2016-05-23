//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema.CodeGeneration.CSharp;

namespace NSwag.CodeGeneration.CodeGenerators.CSharp
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
                Namespace = "MyNamespace"
            };
        }

        /// <summary>Gets or sets the CSharp generator settings.</summary>
        public CSharpGeneratorSettings CSharpGeneratorSettings { get; set; }

        /// <summary>Gets or sets the additional namespace usages.</summary>
        public string[] AdditionalNamespaceUsages { get; set; }
    }
}