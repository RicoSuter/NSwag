//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema.CodeGeneration.CSharp;
using NSwag.CodeGeneration.ClientGenerators.TypeScript;

namespace NSwag.CodeGeneration.ClientGenerators.CSharp
{
    /// <summary>Settings for the <see cref="SwaggerToCSharpGenerator"/>.</summary>
    public class SwaggerToCSharpGeneratorSettings : ClientGeneratorBaseSettings
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerToCSharpGeneratorSettings"/> class.</summary>
        public SwaggerToCSharpGeneratorSettings()
        {
            ClassName = "{controller}Client";
            AdditionalNamespaceUsages = new string[] { };
            CSharpGeneratorSettings = new CSharpGeneratorSettings();
        }

        /// <summary>Gets or sets the CSharp generator settings.</summary>
        public CSharpGeneratorSettings CSharpGeneratorSettings { get; set; }
        
        /// <summary>Gets or sets the class name of the service client.</summary>
        public string ClassName { get; set; }

        /// <summary>Gets or sets the full name of the base class.</summary>
        public string ClientBaseClass { get; set; }

        /// <summary>Gets or sets a value indicating whether to call CreateHttpClientAsync on the base class to create a new HttpClient.</summary>
        public bool UseHttpClientCreationMethod { get; set; }

        /// <summary>Gets or sets the additional namespace usages.</summary>
        public string[] AdditionalNamespaceUsages { get; set; }
    }
}