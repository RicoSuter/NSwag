//-----------------------------------------------------------------------
// <copyright file="AssemblyTypeToSwaggerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi
{
    /// <summary>Settings for the <see cref="AssemblyTypeToSwaggerGenerator"/>.</summary>
    public class AssemblyTypeToSwaggerGeneratorSettings : JsonSchemaGeneratorSettings
    {
        /// <summary>Gets or sets the assembly path.</summary>
        public string AssemblyPath { get; set; }
    }
}