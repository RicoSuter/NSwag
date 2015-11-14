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
                using (var isolated = new AppDomainIsolation<AssemblyLoader>(Path.GetDirectoryName(Settings.AssemblyPath)))
                    return isolated.Object.GetClasses(Settings.AssemblyPath);
            }
            return new string[] { };
        }

        /// <summary>Generates the Swagger definition for the given classes without operations (used for class generation).</summary>
        /// <param name="classNames">The class names.</param>
        /// <returns>The Swagger definition.</returns>
        public SwaggerService Generate(string[] classNames)
        {
            using (var isolated = new AppDomainIsolation<AssemblyLoader>(Path.GetDirectoryName(Settings.AssemblyPath)))
                return SwaggerService.FromJson(isolated.Object.FromAssemblyType(classNames, JsonConvert.SerializeObject(Settings)));
        }

        private class AssemblyLoader : MarshalByRefObject
        {
            internal string FromAssemblyType(string[] classNames, string settingsData)
            {
                var settings = JsonConvert.DeserializeObject<AssemblyTypeToSwaggerGeneratorSettings>(settingsData);

                var generator = new JsonSchemaGenerator(settings);
                var resolver = new SchemaResolver();
                var service = new SwaggerService();

                var assembly = Assembly.LoadFrom(settings.AssemblyPath);
                foreach (var className in classNames)
                {
                    var type = assembly.GetType(className);
                    var schema = generator.Generate<JsonSchema4>(type, resolver);
                    service.Definitions[type.Name] = schema;
                }

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
