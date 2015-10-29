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
        private readonly string _assemblyPath;

        /// <summary>Initializes a new instance of the <see cref="WebApiAssemblyToSwaggerGenerator" /> class.</summary>
        /// <param name="assemblyPath">The assembly path.</param>
        public WebApiAssemblyToSwaggerGenerator(string assemblyPath) : this (assemblyPath, new JsonSchemaGeneratorSettings())
        {
            _assemblyPath = assemblyPath;
        }

        /// <summary>Initializes a new instance of the <see cref="WebApiAssemblyToSwaggerGenerator" /> class.</summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <param name="jsonSchemaGeneratorSettings">The json schema generator settings.</param>
        public WebApiAssemblyToSwaggerGenerator(string assemblyPath, JsonSchemaGeneratorSettings jsonSchemaGeneratorSettings)
        {
            _assemblyPath = assemblyPath;
            JsonSchemaGeneratorSettings = jsonSchemaGeneratorSettings; 
        }

        /// <summary>Gets or sets the JSON Schema generator settings.</summary>
        public JsonSchemaGeneratorSettings JsonSchemaGeneratorSettings { get; set; }

        /// <summary>Gets the available controller classes from the given assembly.</summary>
        /// <returns>The controller classes.</returns>
        public string[] GetControllerClasses()
        {
            if (File.Exists(_assemblyPath))
            {
                using (var isolated = new AppDomainIsolation<AssemblyLoader>(Path.GetDirectoryName(_assemblyPath)))
                    return isolated.Object.GetControllerClasses(_assemblyPath);
            }
            return new string[] { };
        }

        /// <summary>Generates the Swagger definition for the given controller.</summary>
        /// <param name="controllerClassName">The full name of the controller class.</param>
        /// <param name="urlTemplate">The default Web API URL template.</param>
        /// <returns>The Swagger definition.</returns>
        public SwaggerService GenerateForSingleController(string controllerClassName, string urlTemplate)
        {
            using (var isolated = new AppDomainIsolation<AssemblyLoader>(Path.GetDirectoryName(_assemblyPath)))
            {
                var service = isolated.Object.GenerateForSingleController(
                    _assemblyPath, controllerClassName, urlTemplate, JsonConvert.SerializeObject(JsonSchemaGeneratorSettings)); 

                return SwaggerService.FromJson(service);
            }
        }

        /// <summary>Generates the Swagger definition for all controllers in the assembly.</summary>
        /// <param name="urlTemplate">The default Web API URL template.</param>
        /// <returns>The Swagger definition.</returns>
        public SwaggerService GenerateForAssemblyControllers(string urlTemplate)
        {
            using (var isolated = new AppDomainIsolation<AssemblyLoader>(Path.GetDirectoryName(_assemblyPath)))
            {
                var service = isolated.Object.GenerateForAssemblyControllers(
                    _assemblyPath, urlTemplate, JsonConvert.SerializeObject(JsonSchemaGeneratorSettings));

                return SwaggerService.FromJson(service);
            }
        }

        private class AssemblyLoader : MarshalByRefObject
        {
            internal string GenerateForSingleController(string assemblyPath, string controllerClassName, string urlTemplate, string jsonSchemaGeneratorSettingsData)
            {
                var jsonSchemaGeneratorSettings = JsonConvert.DeserializeObject<JsonSchemaGeneratorSettings>(jsonSchemaGeneratorSettingsData);

                var assembly = Assembly.LoadFrom(assemblyPath);
                var type = assembly.GetType(controllerClassName);

                var generator = new WebApiToSwaggerGenerator(urlTemplate, jsonSchemaGeneratorSettings);
                return generator.GenerateForController(type).ToJson();
            }

            internal string GenerateForAssemblyControllers(string assemblyPath, string urlTemplate, string jsonSchemaGeneratorSettingsData)
            {
                var jsonSchemaGeneratorSettings = JsonConvert.DeserializeObject<JsonSchemaGeneratorSettings>(jsonSchemaGeneratorSettingsData);

                var assembly = Assembly.LoadFrom(assemblyPath);
                var controllers = assembly.ExportedTypes
                    .Where(t => t.InheritsFrom("ApiController")).ToArray(); 

                var generator = new WebApiToSwaggerGenerator(urlTemplate, jsonSchemaGeneratorSettings);
                return generator.GenerateForControllers(controllers).ToJson();
            }

            internal string[] GetControllerClasses(string assemblyPath)
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                return assembly.ExportedTypes
                    .Where(t => t.InheritsFrom("ApiController"))
                    .Select(t => t.FullName)
                    .ToArray();
            }
        }
    }
}