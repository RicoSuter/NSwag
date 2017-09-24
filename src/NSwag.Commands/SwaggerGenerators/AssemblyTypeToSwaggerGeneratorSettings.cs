//-----------------------------------------------------------------------
// <copyright file="AssemblyTypeToSwaggerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;
using NJsonSchema.Generation;
using NSwag.SwaggerGeneration;

namespace NSwag.SwaggerGenerators
{
    /// <summary>Settings for the AssemblyTypeToSwaggerGenerator.</summary>
    public class AssemblyTypeToSwaggerGeneratorSettings : JsonSchemaGeneratorSettings, IAssemblySettings
    {
        /// <summary>Initializes a new instance of the <see cref="AssemblyTypeToSwaggerGeneratorSettings"/> class.</summary>
        public AssemblyTypeToSwaggerGeneratorSettings()
        {
            SchemaType = SchemaType.Swagger2;
            AssemblySettings = new AssemblySettings();
        }

        /// <summary>Gets or sets the Web API assembly paths.</summary>
        public AssemblySettings AssemblySettings { get; }
    }
}