//-----------------------------------------------------------------------
// <copyright file="AssemblyTypeToSwaggerGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag.AssemblyLoader.Infrastructure;

namespace NSwag.AssemblyLoader.SwaggerGenerators
{
    /// <summary>Generates a <see cref="SwaggerService"/> from a Web API controller or type which is located in a .NET assembly.</summary>
    public class AssemblyTypeToSwaggerGenerator
    {
        /// <summary>Initializes a new instance of the <see cref="AssemblyTypeToSwaggerGenerator" /> class.</summary>
        /// <param name="settings">The settings.</param>
        public AssemblyTypeToSwaggerGenerator(AssemblyTypeToSwaggerGeneratorSettings settings)
        {
            Settings = settings;
        }

        /// <summary>Gets or sets the generator settings.</summary>
        public AssemblyTypeToSwaggerGeneratorSettings Settings { get; set; }

        /// <summary>Gets the available controller classes from the given assembly.</summary>
        /// <returns>The controller classes.</returns>
        public string[] GetClasses()
        {
            if (File.Exists(Settings.AssemblyPath))
            {
                using (var isolated = new AppDomainIsolation<NetAssemblyLoader>(Path.GetDirectoryName(Settings.AssemblyPath), Settings.AssemblyConfig))
                    return isolated.Object.GetClasses(Settings.AssemblyPath, Settings.ReferencePaths);
            }
            return new string[] { };
        }

        /// <summary>Generates the Swagger definition for the given classes without operations (used for class generation).</summary>
        /// <param name="classNames">The class names.</param>
        /// <returns>The Swagger definition.</returns>
        public SwaggerService Generate(string[] classNames)
        {
            using (var isolated = new AppDomainIsolation<NetAssemblyLoader>(Path.GetDirectoryName(Settings.AssemblyPath), Settings.AssemblyConfig))
                return SwaggerService.FromJson(isolated.Object.FromAssemblyType(classNames, JsonConvert.SerializeObject(Settings)));
        }

        private class NetAssemblyLoader : AssemblyLoader
        {
            internal string FromAssemblyType(string[] classNames, string settingsData)
            {
                var settings = JsonConvert.DeserializeObject<AssemblyTypeToSwaggerGeneratorSettings>(settingsData);
                RegisterReferencePaths(settings.ReferencePaths);

                var generator = new JsonSchemaGenerator(settings);
                var resolver = new SchemaResolver();
                var service = new SwaggerService();

                var assembly = Assembly.LoadFrom(settings.AssemblyPath);
                foreach (var className in classNames)
                {
                    var type = assembly.GetType(className);
                    var schema = generator.Generate(type, resolver);
                    service.Definitions[type.Name] = schema;
                }

                return service.ToJson();
            }

            internal string[] GetClasses(string assemblyPath, IEnumerable<string> referencePaths)
            {
                RegisterReferencePaths(referencePaths);

                var assembly = Assembly.LoadFrom(assemblyPath);
                return assembly.ExportedTypes
                    .Select(t => t.FullName)
                    .OrderBy(t => t)
                    .ToArray();
            }
        }
    }
}
