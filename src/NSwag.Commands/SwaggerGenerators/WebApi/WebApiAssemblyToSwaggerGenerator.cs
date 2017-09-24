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
using System.Threading.Tasks;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag.AssemblyLoader;
using NSwag.AssemblyLoader.Utilities;
using NSwag.Commands.SwaggerGenerators;
using NSwag.SwaggerGeneration.WebApi;

#if !FullNet
using NJsonSchema.Infrastructure;
using System.Runtime.Loader;
#endif

namespace NSwag.SwaggerGenerators.WebApi
{
    /// <summary>Generates a <see cref="SwaggerDocument"/> from a Web API controller or type which is located in a .NET assembly.</summary>
    public class WebApiAssemblyToSwaggerGenerator
    {
        /// <summary>Gets the settings.</summary>
        public WebApiAssemblyToSwaggerGeneratorSettings Settings { get; }

        /// <summary>Initializes a new instance of the <see cref="WebApiAssemblyToSwaggerGenerator"/> class.</summary>
        /// <param name="settings">The generator settings.</param>
        public WebApiAssemblyToSwaggerGenerator(WebApiAssemblyToSwaggerGeneratorSettings settings)
        {
            Settings = settings;
        }

        /// <summary>Gets the available controller classes from the given assembly.</summary>
        /// <returns>The controller classes.</returns>
        /// <exception cref="FileNotFoundException">The assembly could not be found.</exception>
        /// <exception cref="FileNotFoundException">The assembly config file could not be found..</exception>
        /// <exception cref="InvalidOperationException">No assembly paths have been provided.</exception>
        public string[] GetExportedControllerClassNames()
        {
            if (Settings.AssemblySettings.AssemblyPaths == null || Settings.AssemblySettings.AssemblyPaths.Length == 0)
                throw new InvalidOperationException("No assembly paths have been provided.");

            if (!File.Exists(Settings.AssemblySettings.AssemblyPaths.First()))
                throw new FileNotFoundException("The assembly could not be found.", Settings.AssemblySettings.AssemblyPaths.First());

            if (!string.IsNullOrEmpty(Settings.AssemblySettings.AssemblyConfig) && !File.Exists(Settings.AssemblySettings.AssemblyConfig))
                throw new FileNotFoundException("The assembly config file could not be found.", Settings.AssemblySettings.AssemblyConfig);

            var assemblyDirectory = Path.GetDirectoryName(Path.GetFullPath(Settings.AssemblySettings.AssemblyPaths.First())); 
            using (var isolated = new AppDomainIsolation<WebApiAssemblyLoader>(assemblyDirectory, Settings.AssemblySettings.AssemblyConfig, AssemblyLoaderUtilities.GetBindingRedirects(), AssemblyLoaderUtilities.GetAssemblies()))
                return isolated.Object.GetControllerClasses(Settings.AssemblySettings.AssemblyPaths, GetAllReferencePaths(Settings));
        }

        /// <summary>Generates the Swagger definition for all controllers in the assembly.</summary>
        /// <param name="controllerClassNames">The controller class names.</param>
        /// <exception cref="InvalidOperationException">No assembly paths have been provided.</exception>
        /// <returns>The Swagger definition.</returns>
        public async Task<SwaggerDocument> GenerateForControllersAsync(IEnumerable<string> controllerClassNames)
        {
            if (!(Settings.SchemaNameGenerator is DefaultSchemaNameGenerator))
                throw new InvalidOperationException("The SchemaNameGenerator cannot be customized when loading types from external assemblies.");

            if (!(Settings.TypeNameGenerator is DefaultTypeNameGenerator))
                throw new InvalidOperationException("The TypeNameGenerator cannot be customized when loading types from external assemblies.");

            var assemblyDirectory = Path.GetDirectoryName(Path.GetFullPath(Settings.AssemblySettings.AssemblyPaths.First()));
            using (var isolated = new AppDomainIsolation<WebApiAssemblyLoader>(assemblyDirectory, Settings.AssemblySettings.AssemblyConfig, AssemblyLoaderUtilities.GetBindingRedirects(), AssemblyLoaderUtilities.GetAssemblies()))
            {
                var document = await Task.Run(() => isolated.Object.GenerateForControllers(controllerClassNames, JsonConvert.SerializeObject(Settings))).ConfigureAwait(false);
                return await SwaggerDocument.FromJsonAsync(document).ConfigureAwait(false);
            }
        }

        private static string[] GetAllReferencePaths(WebApiAssemblyToSwaggerGeneratorSettings settings)
        {
            return settings.AssemblySettings.AssemblyPaths.Select(p => Path.GetDirectoryName(PathUtilities.MakeAbsolutePath(p, Directory.GetCurrentDirectory())))
                .Concat(settings.AssemblySettings.ReferencePaths)
                .Distinct()
                .ToArray();
        }

        private class WebApiAssemblyLoader : AssemblyLoader.AssemblyLoader
        {
            /// <exception cref="InvalidOperationException">No assembly paths have been provided.</exception>
            internal string GenerateForControllers(IEnumerable<string> controllerClassNames, string settingsData)
            {
                return GenerateForControllersAsync(controllerClassNames, settingsData).GetAwaiter().GetResult();
            }

            private async Task<string> GenerateForControllersAsync(IEnumerable<string> controllerClassNames, string settingsData)
            {
                var settings = JsonConvert.DeserializeObject<WebApiAssemblyToSwaggerGeneratorSettings>(settingsData);

                RegisterReferencePaths(GetAllReferencePaths(settings));
                var controllers = await GetControllerTypesAsync(controllerClassNames, settings);

                var generator = new WebApiToSwaggerGenerator(settings);
                var document = await generator.GenerateForControllersAsync(controllers).ConfigureAwait(false);
                return document.ToJson();
            }

            /// <exception cref="InvalidOperationException">No assembly paths have been provided.</exception>
#pragma warning disable 1998
            private async Task<IEnumerable<Type>> GetControllerTypesAsync(IEnumerable<string> controllerClassNames, WebApiAssemblyToSwaggerGeneratorSettings settings)
#pragma warning restore 1998
            {
                if (settings.AssemblySettings.AssemblyPaths == null || settings.AssemblySettings.AssemblyPaths.Length == 0)
                    throw new InvalidOperationException("No assembly paths have been provided.");

#if FullNet
                var assemblies = PathUtilities.ExpandFileWildcards(settings.AssemblySettings.AssemblyPaths)
                    .Select(path => Assembly.LoadFrom(path)).ToArray();
#else
                var currentDirectory = await DynamicApis.DirectoryGetCurrentDirectoryAsync().ConfigureAwait(false);
                var assemblies = PathUtilities.ExpandFileWildcards(settings.AssemblySettings.AssemblyPaths)
                    .Select(path => Context.LoadFromAssemblyPath(PathUtilities.MakeAbsolutePath(path, currentDirectory))).ToArray();
#endif

                var allExportedClassNames = assemblies.SelectMany(a => a.ExportedTypes).Select(t => t.FullName).ToList();
                var matchedControllerClassNames = controllerClassNames
                    .SelectMany(n => PathUtilities.FindWildcardMatches(n, allExportedClassNames, '.'))
                    .Distinct();

                var controllerClassNamesWithoutWildcard = controllerClassNames.Where(n => !n.Contains("*")).ToArray();
                if (controllerClassNamesWithoutWildcard.Any(n => !matchedControllerClassNames.Contains(n)))
                    throw new TypeLoadException("Unable to load type for controllers: " + string.Join(", ", controllerClassNamesWithoutWildcard));

                var controllerTypes = new List<Type>();
                foreach (var className in matchedControllerClassNames)
                {
                    var controllerType = assemblies.Select(a => a.GetType(className)).FirstOrDefault(t => t != null);
                    if (controllerType != null)
                        controllerTypes.Add(controllerType);
                    else
                        throw new TypeLoadException("Unable to load type for controller: " + className);
                }
                return controllerTypes;
            }

            internal string[] GetControllerClasses(string[] assemblyPaths, IEnumerable<string> referencePaths)
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
                    .SelectMany(WebApiToSwaggerGenerator.GetControllerClasses)
                    .Select(t => t.FullName)
                    .OrderBy(c => c)
                    .ToArray();
            }
        }
    }
}