//-----------------------------------------------------------------------
// <copyright file="WebApiAssemblyToSwaggerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSwag.CodeGeneration.Utilities;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi
{
    /// <summary>Settings for the <see cref="WebApiAssemblyToSwaggerGenerator"/>.</summary>
    public class WebApiAssemblyToSwaggerGeneratorSettings : WebApiToSwaggerGeneratorSettings
    {
        /// <summary>Initializes a new instance of the <see cref="WebApiAssemblyToSwaggerGeneratorSettings"/> class.</summary>
        public WebApiAssemblyToSwaggerGeneratorSettings()
        {
            AssemblyPaths = new string[] { };
            ReferencePaths = new string[] { };
        }

        /// <summary>Gets or sets the Web API assembly paths.</summary>
        public string[] AssemblyPaths { get; set; }

        /// <summary>Gets or sets the path to the assembly App.config or Web.config (optional).</summary>
        public string AssemblyConfig { get; set; }

        /// <summary>Gets ot sets the paths where to search for referenced assemblies</summary>
        public string[] ReferencePaths { get; set; }

        /// <summary>Gets all reference paths.</summary>
        public IEnumerable<string> AllReferencePaths
        {
            get
            {
                return AssemblyPaths.Select(p => Path.GetDirectoryName(PathUtilities.MakeAbsolutePath(p, Directory.GetCurrentDirectory())))
                    .Concat(ReferencePaths)
                    .Distinct();
            }
        }
    }
}