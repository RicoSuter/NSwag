//-----------------------------------------------------------------------
// <copyright file="WebApiAssemblyToSwaggerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.SwaggerGeneration.AspNetCore;

namespace NSwag.Commands.AspNetCore
{
    /// <summary>Settings for the WebApiAssemblyToSwaggerGenerator.</summary>
    internal class AspNetCoreToSwaggerGeneratorCommandSettings : AspNetCoreToSwaggerGeneratorSettings
    {
        /// <summary>Gets or sets the additional document processor type names in the form 'assemblyName:fullTypeName' or 'fullTypeName' which are instantiated during generation.</summary>
        public string[] DocumentProcessorTypes { get; set; }

        /// <summary>Gets or sets the additional operation processor type names in the form 'assemblyName:fullTypeName' or 'fullTypeName' which are instantiated during generation.</summary>
        public string[] OperationProcessorTypes { get; set; }
    }
}