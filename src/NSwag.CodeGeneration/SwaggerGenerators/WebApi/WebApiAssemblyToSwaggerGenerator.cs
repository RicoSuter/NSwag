//-----------------------------------------------------------------------
// <copyright file="WebApiAssemblyToSwaggerGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NSwag.CodeGeneration.Infrastructure;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi
{
    /// <summary>Generates a <see cref="SwaggerService"/> from a Web API controller or type which is located in a .NET assembly.</summary>
    public class WebApiAssemblyToSwaggerGenerator
    {
        private readonly string _assemblyPath;

        /// <summary>Initializes a new instance of the <see cref="WebApiAssemblyToSwaggerGenerator"/> class.</summary>
        /// <param name="assemblyPath">The assembly path.</param>
        public WebApiAssemblyToSwaggerGenerator(string assemblyPath)
        {
            _assemblyPath = assemblyPath;
        }

        /// <summary>Gets the available controller classes from the given assembly.</summary>
        /// <returns>The controller classes.</returns>
        public string[] GetControllerClasses()
        {
            if (File.Exists(_assemblyPath))
            {
                using (var isolated = new AppDomainIsolation<AssemblyLoader>())
                    return isolated.Object.GetControllerClasses(_assemblyPath);
            }
            return new string[] { };
        }

        /// <summary>Generates the Swagger definition for the given controller.</summary>
        /// <param name="controllerClassName">The full name of the controller class.</param>
        /// <param name="urlTemplate">The default Web API URL template.</param>
        /// <returns>The Swagger definition.</returns>
        public SwaggerService Generate(string controllerClassName, string urlTemplate)
        {
            using (var isolated = new AppDomainIsolation<AssemblyLoader>())
                return SwaggerService.FromJson(isolated.Object.FromWebApiAssembly(_assemblyPath, controllerClassName, urlTemplate));
        }

        private class AssemblyLoader : MarshalByRefObject
        {
            internal string FromWebApiAssembly(string assemblyPath, string controllerClassName, string urlTemplate)
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                var type = assembly.GetType(controllerClassName);

                var generator = new WebApiToSwaggerGenerator(urlTemplate);
                return generator.Generate(type).ToJson();
            }
            
            internal string[] GetControllerClasses(string assemblyPath)
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                return assembly.ExportedTypes
                    .Where(t =>
                    {
                        var baseType = t.BaseType;
                        while (baseType != null)
                        {
                            if (baseType.Name == "ApiController")
                                return true;
                            baseType = baseType.BaseType;
                        }
                        return false; 
                    })
                    .Select(t => t.FullName)
                    .ToArray();
            }
        }
    }
}