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
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag.Annotations;
using NSwag.AssemblyLoader;
using NSwag.AssemblyLoader.Utilities;
using NSwag.Commands.SwaggerGenerators;
using NSwag.SwaggerGeneration;
using NSwag.SwaggerGeneration.WebApi;

#if !FullNet
using NJsonSchema;
using NJsonSchema.Infrastructure;
using System.Runtime.Loader;
using NSwag.SwaggerGeneration;
#endif

namespace NSwag.SwaggerGenerators
{
    /// <summary>Generates a <see cref="SwaggerDocument"/> from a Web API controller or type which is located in a .NET assembly.</summary>
    public class AssemblyTypeToSwaggerGenerator
    {
        /// <summary>Gets the settings.</summary>
        public AssemblyTypeToSwaggerGeneratorSettings Settings { get; }

        /// <summary>Initializes a new instance of the <see cref="AssemblyTypeToSwaggerGenerator"/> class.</summary>
        /// <param name="settings">The settings.</param>
        public AssemblyTypeToSwaggerGenerator(AssemblyTypeToSwaggerGeneratorSettings settings)
        {
            Settings = settings;
        }

        /// <summary>Gets the available controller classes from the given assembly.</summary>
        /// <returns>The controller classes.</returns>
        public string[] GetExportedClassNames()
        {
            var assemblyDirectory = Path.GetDirectoryName(Path.GetFullPath(Settings.AssemblySettings.AssemblyPaths.First()));
            using (var isolated = new AppDomainIsolation<NetAssemblyLoader>(
                assemblyDirectory, Settings.AssemblySettings.AssemblyConfig, AssemblyLoaderUtilities.GetBindingRedirects(), AssemblyLoaderUtilities.GetAssemblies()))
                return isolated.Object.GetExportedClassNames(Settings.AssemblySettings.AssemblyPaths, GetAllReferencePaths(Settings));
        }

        /// <summary>Generates the Swagger definition for the given classes without operations (used for class generation).</summary>
        /// <param name="classNames">The class names.</param>
        /// <returns>The Swagger definition.</returns>
        public async Task<SwaggerDocument> GenerateAsync(string[] classNames)
        {
            var assemblyPath = Settings.AssemblySettings.AssemblyPaths.First();
            var assemblyDirectory = Path.GetDirectoryName(Path.GetFullPath(assemblyPath));
            using (var isolated = new AppDomainIsolation<NetAssemblyLoader>(
                assemblyDirectory, Settings.AssemblySettings.AssemblyConfig, AssemblyLoaderUtilities.GetBindingRedirects(), AssemblyLoaderUtilities.GetAssemblies()))
            {
                var json = await Task.Run(() => isolated.Object.FromAssemblyType(classNames, JsonConvert.SerializeObject(Settings))).ConfigureAwait(false);
                return await SwaggerDocument.FromJsonAsync(json).ConfigureAwait(false);
            }
        }

        private static string[] GetAllReferencePaths(AssemblyTypeToSwaggerGeneratorSettings settings)
        {
            var assemblyPath = settings.AssemblySettings.AssemblyPaths.First();
            return new[] { Path.GetDirectoryName(PathUtilities.MakeAbsolutePath(assemblyPath, Directory.GetCurrentDirectory())) }
                .Concat(settings.AssemblySettings.ReferencePaths)
                .Distinct()
                .ToArray();
        }

        private class NetAssemblyLoader : AssemblyLoader.AssemblyLoader
        {
            internal string FromAssemblyType(string[] classNames, string settingsData)
            {
                return FromAssemblyTypeAsync(classNames, settingsData).GetAwaiter().GetResult();
            }

            private async Task<string> FromAssemblyTypeAsync(string[] classNames, string settingsData)
            {
                var document = new SwaggerDocument();
                var settings = JsonConvert.DeserializeObject<AssemblyTypeToSwaggerGeneratorSettings>(settingsData);

                RegisterReferencePaths(GetAllReferencePaths(settings));

                var generator = new JsonSchemaGenerator(settings);
                var schemaResolver = new SwaggerSchemaResolver(document, settings);

#if FullNet
                var assemblies = PathUtilities.ExpandFileWildcards(settings.AssemblySettings.AssemblyPaths)
                    .Select(path => Assembly.LoadFrom(path)).ToArray();
#else
                var currentDirectory = await DynamicApis.DirectoryGetCurrentDirectoryAsync().ConfigureAwait(false);
                var assemblies = PathUtilities.ExpandFileWildcards(settings.AssemblySettings.AssemblyPaths)
                    .Select(path => Context.LoadFromAssemblyPath(PathUtilities.MakeAbsolutePath(path, currentDirectory))).ToArray();
#endif

                var allExportedClassNames = assemblies.SelectMany(a => a.ExportedTypes).Select(t => t.FullName).ToList();
                var matchedClassNames = classNames
                    .SelectMany(n => PathUtilities.FindWildcardMatches(n, allExportedClassNames, '.'))
                    .Distinct();

                foreach (var className in matchedClassNames)
                {
                    var type = assemblies.Select(a => a.GetType(className)).FirstOrDefault(t => t != null);
                    await generator.GenerateAsync(type, schemaResolver).ConfigureAwait(false);
                }

                return document.ToJson();
            }

            internal string[] GetExportedClassNames(string[] assemblyPaths, IEnumerable<string> referencePaths)
            {
                RegisterReferencePaths(referencePaths);

#if FullNet
                return PathUtilities.ExpandFileWildcards(assemblyPaths)
                    .Select(Assembly.LoadFrom)
#else
                var currentDirectory = DynamicApis.DirectoryGetCurrentDirectoryAsync().GetAwaiter().GetResult();
                return PathUtilities.ExpandFileWildcards(assemblyPaths)
                    .Select(p => Context.LoadFromAssemblyPath(PathUtilities.MakeAbsolutePath(p, currentDirectory)))
#endif
                    .SelectMany(a => a.ExportedTypes)
                    .Select(t => t.FullName)
                    .OrderBy(c => c)
                    .ToArray();
            }
        }
    }
}
