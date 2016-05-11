//-----------------------------------------------------------------------
// <copyright file="WebApiAssemblyToSwaggerGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using NSwag.AssemblyLoader.Infrastructure;
using NSwag.CodeGeneration.Infrastructure;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.AssemblyLoader.SwaggerGenerators.WebApi
{
    /// <summary>Generates a <see cref="SwaggerService"/> from a Web API controller or type which is located in a .NET assembly.</summary>
    public class WebApiAssemblyToSwaggerGenerator
    {
        /// <summary>Initializes a new instance of the <see cref="WebApiAssemblyToSwaggerGenerator" /> class.</summary>
        /// <param name="settings">The generator settings.</param>
        public WebApiAssemblyToSwaggerGenerator(WebApiAssemblyToSwaggerGeneratorSettings settings)
        {
            Settings = settings;
        }

        /// <summary>Gets or sets the generator settings.</summary>
        public WebApiAssemblyToSwaggerGeneratorSettings Settings { get; set; }

        /// <summary>Gets the available controller classes from the given assembly.</summary>
        /// <returns>The controller classes.</returns>
        public string[] GetControllerClasses()
        {
            if (File.Exists(Settings.AssemblyPath))
            {
                using (var isolated = new AppDomainIsolation<WebApiAssemblyLoader>(Path.GetDirectoryName(Settings.AssemblyPath), Settings.AssemblyConfig))
                    return isolated.Object.GetControllerClasses(Settings.AssemblyPath, Settings.ReferencePaths);
            }
            return new string[] { };
        }

        /// <summary>Generates the Swagger definition for the given controller.</summary>
        /// <param name="controllerClassName">The full name of the controller class.</param>
        /// <returns>The Swagger definition.</returns>
        public SwaggerService GenerateForController(string controllerClassName)
        {
            using (var isolated = new AppDomainIsolation<WebApiAssemblyLoader>(Path.GetDirectoryName(Settings.AssemblyPath), Settings.AssemblyConfig))
            {
                var service = isolated.Object.GenerateForController(controllerClassName, JsonConvert.SerializeObject(Settings));
                return SwaggerService.FromJson(service);
            }
        }

        /// <summary>Generates the Swagger definition for all controllers in the assembly.</summary>
        /// <param name="controllerClassNames">The controller class names.</param>
        /// <returns>The Swagger definition.</returns>
        public SwaggerService GenerateForControllers(IEnumerable<string> controllerClassNames)
        {
            using (var isolated = new AppDomainIsolation<WebApiAssemblyLoader>(Path.GetDirectoryName(Settings.AssemblyPath), Settings.AssemblyConfig))
            {
                var service = isolated.Object.GenerateForControllers(controllerClassNames, JsonConvert.SerializeObject(Settings));
                return SwaggerService.FromJson(service);
            }
        }

        private class WebApiAssemblyLoader : AssemblyLoader
        {
            internal string GenerateForController(string controllerClassName, string settingsData)
            {
                var settings = JsonConvert.DeserializeObject<WebApiAssemblyToSwaggerGeneratorSettings>(settingsData);
                RegisterReferencePaths(settings.ReferencePaths);

                IEnumerable<Type> controllers = GetControllerTypes(new string[] { controllerClassName }, settings);
                var type = controllers.First();

                var generator = new WebApiToSwaggerGenerator(settings);
                return generator.GenerateForController(type).ToJson();
            }

            internal string GenerateForControllers(IEnumerable<string> controllerClassNames, string settingsData)
            {
                var settings = JsonConvert.DeserializeObject<WebApiAssemblyToSwaggerGeneratorSettings>(settingsData);
                RegisterReferencePaths(settings.ReferencePaths);
                IEnumerable<Type> controllers = GetControllerTypes(controllerClassNames, settings);

                var generator = new WebApiToSwaggerGenerator(settings);
                return generator.GenerateForControllers(controllers).ToJson();
            }

            private IEnumerable<Type> GetControllerTypes(IEnumerable<string> controllerClassNames, WebApiAssemblyToSwaggerGeneratorSettings settings)
            {
                var assembly = Assembly.LoadFrom(settings.AssemblyPath);
                var types = assembly.GetTypes();

                var controllerTypes = new List<Type>();
                foreach (var className in controllerClassNames)
                {
                    try
                    {
                        var type = types.First(t => t.FullName == className || t.Name == className);
                        controllerTypes.Add(type);
                    }
                    catch
                    {
                        throw new TypeLoadException("Unable to load type for controller: " + className);
                    }
                }
                return controllerTypes;
            }

            internal string[] GetControllerClasses(string assemblyPath, IEnumerable<string> referencePaths)
            {
                RegisterReferencePaths(referencePaths);
                var assembly = Assembly.LoadFrom(assemblyPath);
                return assembly.ExportedTypes
                    .Where(t => t.Name.EndsWith("Controller") || t.InheritsFrom("ApiController") || t.InheritsFrom("Controller"))
                    .Select(t => t.FullName)
                    .ToArray();
            }
        }
    }
}