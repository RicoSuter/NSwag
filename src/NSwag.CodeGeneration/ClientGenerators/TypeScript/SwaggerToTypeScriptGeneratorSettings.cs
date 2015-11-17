//-----------------------------------------------------------------------
// <copyright file="SwaggerToTypeScriptGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.ClientGenerators.TypeScript
{
    /// <summary>Settings for the <see cref="SwaggerToTypeScriptGenerator"/>.</summary>
    public class SwaggerToTypeScriptGeneratorSettings : ClientGeneratorBaseSettings
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerToTypeScriptGeneratorSettings"/> class.</summary>
        public SwaggerToTypeScriptGeneratorSettings()
        {
            ClassName = "{controller}Client";
            AsyncType = TypeScriptAsyncType.Callbacks;
        }

        /// <summary>Gets or sets the class name of the service client.</summary>
        public string ClassName { get; set; }

        /// <summary>Gets or sets the type of the asynchronism handling.</summary>
        public TypeScriptAsyncType AsyncType { get; set; }
    }
}