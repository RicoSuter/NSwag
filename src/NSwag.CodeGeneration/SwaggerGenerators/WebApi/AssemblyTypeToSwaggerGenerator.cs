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
    /// <summary>Generates a <see cref="SwaggerService"/> from a Web API controller which is located in a .NET assembly.</summary>
    public class AssemblyTypeToSwaggerGenerator
    {
        /// <summary>Gets the available controller classes from the given assembly.</summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <returns>The controller classes.</returns>
        public static string[] GetControllerClasses(string assemblyPath)
        {
            if (File.Exists(assemblyPath))
            {
                using (var isolated = new AppDomainIsolation<NSwagServiceLoader>())
                    return isolated.Object.GetControllerClasses(assemblyPath);
            }
            return new string[] { };
        }

        /// <summary>Gets the available controller classes from the given assembly.</summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <returns>The controller classes.</returns>
        public static string[] GetClasses(string assemblyPath)
        {
            if (File.Exists(assemblyPath))
            {
                using (var isolated = new AppDomainIsolation<NSwagServiceLoader>())
                    return isolated.Object.GetClasses(assemblyPath);
            }
            return new string[] { };
        }

        /// <summary>Generates the Swagger definition for the given controller.</summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <param name="controllerClassName">The full name of the controller class.</param>
        /// <param name="urlTemplate">The default Web API URL template.</param>
        /// <returns>The Swagger definition.</returns>
        public static string FromWebApiAssembly(string assemblyPath, string controllerClassName, string urlTemplate)
        {
            using (var isolated = new AppDomainIsolation<NSwagServiceLoader>())
                return isolated.Object.FromWebApiAssembly(assemblyPath, controllerClassName, urlTemplate);
        }

        /// <summary>Generates the Swagger definition for the given classes without operations (used for class generation).</summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <param name="className">The class name.</param>
        /// <returns>The Swagger definition.</returns>
        public static string FromAssemblyType(string assemblyPath, string className)
        {
            using (var isolated = new AppDomainIsolation<NSwagServiceLoader>())
                return isolated.Object.FromAssemblyType(assemblyPath, className);
        }

        private class NSwagServiceLoader : MarshalByRefObject
        {
            internal string FromWebApiAssembly(string assemblyPath, string controllerClassName, string urlTemplate)
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                var type = assembly.GetType(controllerClassName);

                var generator = new WebApiToSwaggerGenerator(urlTemplate);
                var service = generator.Generate(type);
                return service.ToJson();
            }

            internal string FromAssemblyType(string assemblyPath, string className)
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                var type = assembly.GetType(className); 

                var service = new SwaggerService();
                var schema = JsonSchema4.FromType(type);
                service.Definitions[type.Name] = schema;

                return service.ToJson();
            }

            internal string[] GetControllerClasses(string assemblyPath)
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                return assembly.ExportedTypes
                    .Where(t => t.BaseType != null && t.BaseType.Name == "ApiController")
                    .Select(t => t.FullName)
                    .ToArray();
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
