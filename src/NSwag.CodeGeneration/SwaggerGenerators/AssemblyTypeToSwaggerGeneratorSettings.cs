//-----------------------------------------------------------------------
// <copyright file="AssemblyTypeToSwaggerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;
using NJsonSchema.Generation;

namespace NSwag.CodeGeneration.SwaggerGenerators
{
    /// <summary>Settings for the AssemblyTypeToSwaggerGenerator.</summary>
    public class AssemblyTypeToSwaggerGeneratorSettings : JsonSchemaGeneratorSettings
    {
        /// <summary>Initializes a new instance of the <see cref="AssemblyTypeToSwaggerGeneratorSettings"/> class.</summary>
        public AssemblyTypeToSwaggerGeneratorSettings()
        {
            NullHandling = NullHandling.Swagger;
            ReferencePaths = new string[] { };
        }

        /// <summary>Gets or sets the assembly path.</summary>
        public string AssemblyPath { get; set; }

        /// <summary>Gets or sets the path to the assembly App.config or Web.config (optional).</summary>
        public string AssemblyConfig { get; set; }

        /// <summary>Gets ot sets the paths where to search for referenced assemblies</summary>
        public string[] ReferencePaths { get; set; }
    }
}