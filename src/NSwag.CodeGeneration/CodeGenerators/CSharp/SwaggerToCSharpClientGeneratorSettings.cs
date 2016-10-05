//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpClientGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.CodeGenerators.CSharp
{
    /// <summary>Settings for the <see cref="SwaggerToCSharpClientGenerator"/>.</summary>
    public class SwaggerToCSharpClientGeneratorSettings : SwaggerToCSharpGeneratorSettings
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerToCSharpClientGeneratorSettings"/> class.</summary>
        public SwaggerToCSharpClientGeneratorSettings()
        {
            ClassName = "{controller}Client";
            ExceptionClass = "SwaggerException";
        }

        /// <summary>Gets or sets the full name of the base class.</summary>
        public string ClientBaseClass { get; set; }

        /// <summary>Gets or sets the full name of the configuration class (<see cref="ClientBaseClass"/> must be set).</summary>
        public string ConfigurationClass { get; set; }
        
        /// <summary>Gets or sets the name of the exception class (supports the '{controller}' placeholder).</summary>
        public string ExceptionClass { get; set; }

        /// <summary>Gets or sets a value indicating whether to call CreateHttpClientAsync on the base class to create a new HttpClient instance.</summary>
        public bool UseHttpClientCreationMethod { get; set; }

        /// <summary>Gets or sets a value indicating whether to call CreateHttpRequestMessageAsync on the base class to create a new HttpRequestMethod.</summary>
        public bool UseHttpRequestMessageCreationMethod { get; set; }
    }
}
