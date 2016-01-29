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
        /// <summary>Gets or sets the full name of the base class.</summary>
        public string ClientBaseClass { get; set; }

        /// <summary>Gets or sets a value indicating whether to call CreateHttpClientAsync on the base class to create a new HttpClient.</summary>
        public bool UseHttpClientCreationMethod { get; set; }
    }
}