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
using Newtonsoft.Json;
using NJsonSchema;
using NSwag.CodeGeneration.Infrastructure;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi
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
                using (var isolated = new AppDomainIsolation<AssemblyLoader>(Path.GetDirectoryName(Settings.AssemblyPath)))
                    return isolated.Object.GetControllerClasses(Settings.AssemblyPath);
            }
            return new string[] { };
        }

        /// <summary>Generates the Swagger definition for the given controller.</summary>
        /// <param name="controllerClassName">The full name of the controller class.</param>
        /// <returns>The Swagger definition.</returns>
        public SwaggerService GenerateForSingleController(string controllerClassName)
        {
            using (var isolated = new AppDomainIsolation<AssemblyLoader>(Path.GetDirectoryName(Settings.AssemblyPath)))
            {
                var service = isolated.Object.GenerateForSingleController(controllerClassName, JsonConvert.SerializeObject(Settings)); 
                return SwaggerService.FromJson(service);
            }
        }

        /// <summary>Generates the Swagger definition for all controllers in the assembly.</summary>
        /// <returns>The Swagger definition.</returns>
        public SwaggerService GenerateForAssemblyControllers()
        {
            using (var isolated = new AppDomainIsolation<AssemblyLoader>(Path.GetDirectoryName(Settings.AssemblyPath)))
            {
                var service = isolated.Object.GenerateForAssemblyControllers(JsonConvert.SerializeObject(Settings));

                return SwaggerService.FromJson(service);
            }
        }

        private class AssemblyLoader : MarshalByRefObject
        {
            internal string GenerateForSingleController(string controllerClassName, string settingsData)
            {
                var settings = JsonConvert.DeserializeObject<WebApiAssemblyToSwaggerGeneratorSettings>(settingsData);

                var assembly = Assembly.LoadFrom(settings.AssemblyPath);
                var type = assembly.GetType(controllerClassName);

                var generator = new WebApiToSwaggerGenerator(settings);
                return generator.GenerateForController(type).ToJson();
            }

            internal string GenerateForAssemblyControllers(string settingsData)
            {
                var settings = JsonConvert.DeserializeObject<WebApiAssemblyToSwaggerGeneratorSettings>(settingsData);

                var assembly = Assembly.LoadFrom(settings.AssemblyPath);
                var controllers = assembly.ExportedTypes
                    .Where(t => t.InheritsFrom("ApiController") || t.InheritsFrom("Controller")).ToArray(); 

                var generator = new WebApiToSwaggerGenerator(settings);
                return generator.GenerateForControllers(controllers).ToJson();
            }

            internal string[] GetControllerClasses(string assemblyPath)
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                return assembly.ExportedTypes
                    .Where(t => t.InheritsFrom("ApiController") || t.InheritsFrom("Controller"))
                    .Select(t => t.FullName)
                    .ToArray();
            }
        }
    }
}