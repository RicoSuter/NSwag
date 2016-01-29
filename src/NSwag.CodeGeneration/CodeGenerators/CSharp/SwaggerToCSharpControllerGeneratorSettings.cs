//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpClientGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema.CodeGeneration.CSharp;

namespace NSwag.CodeGeneration.CodeGenerators.CSharp
{
    /// <summary>Settings for the <see cref="SwaggerToCSharpControllerGenerator"/>.</summary>
    public class SwaggerToCSharpControllerGeneratorSettings : SwaggerToCSharpGeneratorSettings
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerToCSharpControllerGeneratorSettings"/> class.</summary>
        public SwaggerToCSharpControllerGeneratorSettings()
        {
            ClassName = "{controller}";
        }

        /// <summary>Gets or sets the full name of the base class.</summary>
        public string ControllerBaseClass { get; set; }
    }
}