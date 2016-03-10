//-----------------------------------------------------------------------
// <copyright file="WebApiAssemblyToSwaggerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi
{
    /// <summary>Settings for the <see cref="WebApiAssemblyToSwaggerGenerator"/>.</summary>
    public class WebApiAssemblyToSwaggerGeneratorSettings : WebApiToSwaggerGeneratorSettings
    {
        /// <summary>Gets or sets the Web API assembly path.</summary>
        public string AssemblyPath { get; set; }

        /// <summary>Gets ot sets the paths where to search for referenced assemblies</summary>
        public string[] ReferencePaths { get; set; }
    }
}