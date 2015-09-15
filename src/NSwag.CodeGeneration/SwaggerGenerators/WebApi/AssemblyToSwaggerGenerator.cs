//-----------------------------------------------------------------------
// <copyright file="AssemblyTypeToSwaggerGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NJsonSchema;
using NSwag.CodeGeneration.Infrastructure;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi
{
    /// <summary>Generates a <see cref="SwaggerService"/> from a Web API controller or type which is located in a .NET assembly.</summary>
    public class AssemblyToSwaggerGenerator
    {
        private readonly string _assemblyPath;

        /// <summary>Initializes a new instance of the <see cref="AssemblyToSwaggerGenerator"/> class.</summary>
        /// <param name="assemblyPath">The assembly path.</param>
        public AssemblyToSwaggerGenerator(string assemblyPath)
        {
            _assemblyPath = assemblyPath; 
        }

        /// <summary>Gets the available controller classes from the given assembly.</summary>
        /// <returns>The controller classes.</returns>
        public string[] GetClasses()
        {
            if (File.Exists(_assemblyPath))
            {
                using (var isolated = new AppDomainIsolation<NSwagServiceLoader>())
                    return isolated.Object.GetClasses(_assemblyPath);
            }
            return new string[] { };
        }
        
        /// <summary>Generates the Swagger definition for the given classes without operations (used for class generation).</summary>
        /// <param name="className">The class name.</param>
        /// <returns>The Swagger definition.</returns>
        public SwaggerService FromAssemblyType(string className)
        {
            using (var isolated = new AppDomainIsolation<NSwagServiceLoader>())
                return SwaggerService.FromJson(isolated.Object.FromAssemblyType(_assemblyPath, className));
        }

        private class NSwagServiceLoader : MarshalByRefObject
        {
            internal string FromAssemblyType(string assemblyPath, string className)
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                var type = assembly.GetType(className); 

                var service = new SwaggerService();
                var schema = JsonSchema4.FromType(type);
                service.Definitions[type.Name] = schema;
                return service.ToJson();
            }

            internal string[] GetClasses(string assemblyPath)
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                return assembly.ExportedTypes
                    .Select(t => t.FullName)
                    .OrderBy(t => t)
                    .ToArray();
            }
        }
    }
}
