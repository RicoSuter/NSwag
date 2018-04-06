//-----------------------------------------------------------------------
// <copyright file="WebApiAssemblyToSwaggerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.SwaggerGeneration.AspNetCore;

namespace NSwag.Commands.SwaggerGeneration.AspNetCore
{
    /// <summary>Settings for the AspNetCoreToSwaggerGeneratorCommand and AspNetCoreToSwaggerGeneratorCommandEntryPoint.</summary>
    public class AspNetCoreToSwaggerGeneratorCommandSettings : AspNetCoreToSwaggerGeneratorSettings
    {
        public string ApplicationName { get; set; }

        public string Output { get; set; }

        /// <summary>Gets or sets the additional document processor type names in the form 'assemblyName:fullTypeName' or 'fullTypeName' which are instantiated during generation.</summary>
        public string[] DocumentProcessorTypes { get; set; }

        /// <summary>Gets or sets the additional operation processor type names in the form 'assemblyName:fullTypeName' or 'fullTypeName' which are instantiated during generation.</summary>
        public string[] OperationProcessorTypes { get; set; }
    }
}