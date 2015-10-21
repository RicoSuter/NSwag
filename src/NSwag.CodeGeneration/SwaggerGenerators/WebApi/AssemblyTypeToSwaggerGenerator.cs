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
using Newtonsoft.Json;
using NJsonSchema;
using NSwag.CodeGeneration.Infrastructure;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi
{
    /// <summary>Generates a <see cref="SwaggerService"/> from a Web API controller or type which is located in a .NET assembly.</summary>
    public class AssemblyTypeToSwaggerGenerator
    {
        private readonly string _assemblyPath;

        /// <summary>Initializes a new instance of the <see cref="AssemblyTypeToSwaggerGenerator" /> class.</summary>
        /// <param name="assemblyPath">The assembly path.</param>
        public AssemblyTypeToSwaggerGenerator(string assemblyPath) : this(assemblyPath, new JsonSchemaGeneratorSettings())
        {
        }

        /// <summary>Initializes a new instance of the <see cref="AssemblyTypeToSwaggerGenerator" /> class.</summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <param name="jsonSchemaGeneratorSettings">The json schema generator settings.</param>
        public AssemblyTypeToSwaggerGenerator(string assemblyPath, JsonSchemaGeneratorSettings jsonSchemaGeneratorSettings)
        {
            _assemblyPath = assemblyPath;
            JsonSchemaGeneratorSettings = jsonSchemaGeneratorSettings;
        }

        /// <summary>Gets or sets the JSON Schema generator settings.</summary>
        public JsonSchemaGeneratorSettings JsonSchemaGeneratorSettings { get; set; }

        /// <summary>Gets the available controller classes from the given assembly.</summary>
        /// <returns>The controller classes.</returns>
        public string[] GetClasses()
        {
            if (File.Exists(_assemblyPath))
            {
                using (var isolated = new AppDomainIsolation<AssemblyLoader>(Path.GetDirectoryName(_assemblyPath)))
                    return isolated.Object.GetClasses(_assemblyPath);
            }
            return new string[] { };
        }

        /// <summary>Generates the Swagger definition for the given classes without operations (used for class generation).</summary>
        /// <param name="className">The class name.</param>
        /// <returns>The Swagger definition.</returns>
        public SwaggerService Generate(string className)
        {
            using (var isolated = new AppDomainIsolation<AssemblyLoader>(Path.GetDirectoryName(_assemblyPath)))
                return SwaggerService.FromJson(isolated.Object.FromAssemblyType(_assemblyPath, className, JsonConvert.SerializeObject(JsonSchemaGeneratorSettings)));
        }

        private class AssemblyLoader : MarshalByRefObject
        {
            internal string FromAssemblyType(string assemblyPath, string className, string jsonSchemaGeneratorSettingsData)
            {
                var jsonSchemaGeneratorSettings = JsonConvert.DeserializeObject<JsonSchemaGeneratorSettings>(jsonSchemaGeneratorSettingsData);

                var assembly = Assembly.LoadFrom(assemblyPath);
                var type = assembly.GetType(className);

                var service = new SwaggerService();
                var schema = JsonSchema4.FromType(type, jsonSchemaGeneratorSettings);
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
