//-----------------------------------------------------------------------
// <copyright file="NSwagSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.CodeGeneration.ClientGenerators.CSharp;
using NSwag.CodeGeneration.ClientGenerators.TypeScript;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwagStudio
{
    public class NSwagSettings
    {
        public NSwagSettings()
        {
            WebApiAssemblyToSwaggerGeneratorSettings = new WebApiAssemblyToSwaggerGeneratorSettings();
            AssemblyTypeToSwaggerGeneratorSettings = new AssemblyTypeToSwaggerGeneratorSettings();

            SwaggerToCSharpGeneratorSettings = new SwaggerToCSharpGeneratorSettings();
            SwaggerToTypeScriptGeneratorSettings = new SwaggerToTypeScriptGeneratorSettings();
        }

        public WebApiAssemblyToSwaggerGeneratorSettings WebApiAssemblyToSwaggerGeneratorSettings { get; set; }

        public AssemblyTypeToSwaggerGeneratorSettings AssemblyTypeToSwaggerGeneratorSettings { get; set; }

        public SwaggerToCSharpGeneratorSettings SwaggerToCSharpGeneratorSettings { get; set; }

        public SwaggerToTypeScriptGeneratorSettings SwaggerToTypeScriptGeneratorSettings { get; set; }
    }
}
