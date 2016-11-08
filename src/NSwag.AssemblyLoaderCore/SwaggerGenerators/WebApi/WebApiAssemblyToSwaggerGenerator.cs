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
using NSwag.CodeGeneration.Infrastructure;
using NSwag.CodeGeneration.Utilities;

#if !FullNet
using System.Runtime.Loader;
#endif

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi
{
    /// <summary>Generates a <see cref="SwaggerDocument"/> from a Web API controller or type which is located in a .NET assembly.</summary>
    public class WebApiAssemblyToSwaggerGenerator : WebApiAssemblyToSwaggerGeneratorBase
    {
        /// <summary>Initializes a new instance of the <see cref="WebApiAssemblyToSwaggerGenerator"/> class.</summary>
        /// <param name="settings">The generator settings.</param>
        public WebApiAssemblyToSwaggerGenerator(WebApiAssemblyToSwaggerGeneratorSettings settings) : base(settings)
        {
        }

        /// <summary>Gets the available controller classes from the given assembly.</summary>
        /// <returns>The controller classes.</returns>
        /// <exception cref="FileNotFoundException">The assembly could not be found.</exception>
        /// <exception cref="FileNotFoundException">The assembly config file could not be found..</exception>
        /// <exception cref="InvalidOperationException">No assembly paths have been provided.</exception>
        public override string[] GetControllerClasses()
        {
            if (Settings.AssemblyPaths == null || Settings.AssemblyPaths.Length == 0)
                throw new InvalidOperationException("No assembly paths have been provided.");

            if (!File.Exists(Settings.AssemblyPaths.First()))
                throw new FileNotFoundException("The assembly could not be found.", Settings.AssemblyPaths.First());

            if (!string.IsNullOrEmpty(Settings.AssemblyConfig) && !File.Exists(Settings.AssemblyConfig))
                throw new FileNotFoundException("The assembly config file could not be found.", Settings.AssemblyConfig);

#if FullNet
            using (var isolated = new AppDomainIsolation<WebApiAssemblyLoader>(Path.GetDirectoryName(Path.GetFullPath(Settings.AssemblyPaths.First())), Settings.AssemblyConfig))
                return isolated.Object.GetControllerClasses(Settings.AssemblyPaths, GetAllReferencePaths(Settings));
#else
            var loader = new WebApiAssemblyLoader();
            return loader.GetControllerClasses(Settings.AssemblyPaths, GetAllReferencePaths(Settings));
#endif
        }

        /// <summary>Generates the Swagger definition for all controllers in the assembly.</summary>
        /// <param name="controllerClassNames">The controller class names.</param>
        /// <exception cref="InvalidOperationException">No assembly paths have been provided.</exception>
        /// <returns>The Swagger definition.</returns>
        public override SwaggerDocument GenerateForControllers(IEnumerable<string> controllerClassNames)
        {
#if FullNet
            using (var isolated = new AppDomainIsolation<WebApiAssemblyLoader>(Path.GetDirectoryName(Path.GetFullPath(Settings.AssemblyPaths.First())), Settings.AssemblyConfig))
            {
                var document = isolated.Object.GenerateForControllers(controllerClassNames, JsonConvert.SerializeObject(Settings));
                return SwaggerDocument.FromJson(document);
            }
#else
            var loader = new WebApiAssemblyLoader();
            var data = loader.GenerateForControllers(controllerClassNames, JsonConvert.SerializeObject(Settings));
            return SwaggerDocument.FromJson(data);
#endif
        }

        private static string[] GetAllReferencePaths(WebApiAssemblyToSwaggerGeneratorSettings settings)
        {
            return settings.AssemblyPaths.Select(p => Path.GetDirectoryName(PathUtilities.MakeAbsolutePath(p, Directory.GetCurrentDirectory())))
                .Concat(settings.ReferencePaths)
                .Distinct()
                .ToArray();
        }

        private class WebApiAssemblyLoader : AssemblyLoader
        {
            /// <exception cref="InvalidOperationException">No assembly paths have been provided.</exception>
            internal string GenerateForControllers(IEnumerable<string> controllerClassNames, string settingsData)
            {
                var settings = JsonConvert.DeserializeObject<WebApiAssemblyToSwaggerGeneratorSettings>(settingsData);

                RegisterReferencePaths(GetAllReferencePaths(settings));
                IEnumerable<Type> controllers = GetControllerTypes(controllerClassNames, settings);

                var generator = new WebApiToSwaggerGenerator(settings);
                return generator.GenerateForControllers(controllers).ToJson();
            }

            /// <exception cref="InvalidOperationException">No assembly paths have been provided.</exception>
            private IEnumerable<Type> GetControllerTypes(IEnumerable<string> controllerClassNames, WebApiAssemblyToSwaggerGeneratorSettings settings)
            {
                if (settings.AssemblyPaths == null || settings.AssemblyPaths.Length == 0)
                    throw new InvalidOperationException("No assembly paths have been provided.");

#if FullNet
                var assemblies = PathUtilities.ExpandFileWildcards(settings.AssemblyPaths)
                    .Select(path => Assembly.LoadFrom(path)).ToArray();
#else
                var assemblies = PathUtilities.ExpandFileWildcards(settings.AssemblyPaths)
                    .Select(path => Context.LoadFromAssemblyPath(path)).ToArray();
#endif

                var controllerTypes = new List<Type>();
                foreach (var className in controllerClassNames)
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

                return PathUtilities.ExpandFileWildcards(assemblyPaths)
#if FullNet
                    .Select(Assembly.LoadFrom)
#else
                    .Select(Context.LoadFromAssemblyPath)
#endif
                    .SelectMany(WebApiToSwaggerGenerator.GetControllerClasses)
                    .Select(t => t.FullName)
                    .OrderBy(c => c)
                    .ToArray();
            }
        }
    }
}