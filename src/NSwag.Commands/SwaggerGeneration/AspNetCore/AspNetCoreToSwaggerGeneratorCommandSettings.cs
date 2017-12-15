//-----------------------------------------------------------------------
// <copyright file="WebApiAssemblyToSwaggerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema.Generation;

namespace NSwag.SwaggerGeneration.AspNetCore
{
    /// <summary>Settings for the WebApiAssemblyToSwaggerGenerator.</summary>
    public class AspNetCoreToSwaggerGeneratorCommandSettings : JsonSchemaGeneratorSettings
    {
        /// <summary>Gets or sets the additional document processor type names in the form 'assemblyName:fullTypeName' or 'fullTypeName' which are instantiated during generation.</summary>
        public string[] DocumentProcessorTypes { get; set; }

        /// <summary>Gets or sets the additional operation processor type names in the form 'assemblyName:fullTypeName' or 'fullTypeName' which are instantiated during generation.</summary>
        public string[] OperationProcessorTypes { get; set; }

        public string Description { get; set; }

        public string Title { get; set; }

        public string Version { get; set; }

        public string DocumentTemplate { get; set; }
    }
}