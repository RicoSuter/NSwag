//-----------------------------------------------------------------------
// <copyright file="NSwagSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NJsonSchema.Infrastructure;
using NSwag.AssemblyLoader.Utilities;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.Commands.SwaggerGeneration.WebApi
{
    /// <summary>The generator.</summary>
    [Command(Name = "webapi2swagger", Description = "Generates a Swagger specification for a controller or controlles contained in a .NET Web API assembly.")]
    public class WebApiToSwaggerCommand : SwaggerGeneratorCommandBase<WebApiToSwaggerGeneratorSettings>
    {
        /// <summary>Initializes a new instance of the <see cref="WebApiToSwaggerCommand"/> class.</summary>
        public WebApiToSwaggerCommand()
        {
            ControllerNames = new string[] { };
        }

        [JsonIgnore]
        [Argument(Name = "Controller", IsRequired = false, Description = "The Web API controller full class name or empty to load all controllers from the assembly.")]
        public string ControllerName
        {
            get => ControllerNames.FirstOrDefault();
            set => ControllerNames = new[] { value };
        }

        [Argument(Name = "Controllers", IsRequired = false, Description = "The Web API controller full class names or empty to load all controllers from the assembly (comma separated).")]
        public string[] ControllerNames { get; set; }

        [Argument(Name = "AspNetCore", IsRequired = false, Description = "Specifies whether the controllers are hosted by ASP.NET Core.")]
        public bool IsAspNetCore
        {
            get => Settings.IsAspNetCore;
            set => Settings.IsAspNetCore = value;
        }

        [Argument(Name = "DefaultUrlTemplate", IsRequired = false, Description = "The Web API default URL template (default for Web API: 'api/{controller}/{id}'; for MVC projects: '{controller}/{action}/{id?}').")]
        public string DefaultUrlTemplate
        {
            get => Settings.DefaultUrlTemplate;
            set => Settings.DefaultUrlTemplate = value;
        }

        [Argument(Name = "AddMissingPathParameters", IsRequired = false, Description = "Specifies whether to add path parameters which are missing in the action method (default: true).")]
        public bool AddMissingPathParameters
        {
            get => Settings.AddMissingPathParameters;
            set => Settings.AddMissingPathParameters = value;
        }

        protected override async Task<string> RunIsolatedAsync(AssemblyLoader.AssemblyLoader assemblyLoader)
        {
            Settings.DocumentTemplate = await GetDocumentTemplateAsync();
            InitializeCustomTypes(assemblyLoader);

            var controllerNames = ControllerNames.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            if (!controllerNames.Any() && AssemblyPaths?.Length > 0)
                controllerNames = GetControllerNames(assemblyLoader).ToList();

            var controllerTypes = await GetControllerTypesAsync(controllerNames, assemblyLoader);

            var generator = new WebApiToSwaggerGenerator(Settings);
            var document = await generator.GenerateForControllersAsync(controllerTypes).ConfigureAwait(false);

            return PostprocessDocument(document);
        }

        private string[] GetControllerNames(AssemblyLoader.AssemblyLoader assemblyLoader)
        {
#if FullNet
            return PathUtilities.ExpandFileWildcards(AssemblyPaths)
                .Select(Assembly.LoadFrom)
#else
            var currentDirectory = DynamicApis.DirectoryGetCurrentDirectoryAsync().GetAwaiter().GetResult();
            return PathUtilities.ExpandFileWildcards(AssemblyPaths)
                .Select(p => assemblyLoader.Context.LoadFromAssemblyPath(PathUtilities.MakeAbsolutePath(p, currentDirectory)))
#endif
                .SelectMany(WebApiToSwaggerGenerator.GetControllerClasses)
                .Select(t => t.FullName)
                .OrderBy(c => c)
                .ToArray();
        }

        private async Task<IEnumerable<Type>> GetControllerTypesAsync(IEnumerable<string> controllerNames, AssemblyLoader.AssemblyLoader assemblyLoader)
#pragma warning restore 1998
        {
            if (AssemblyPaths == null || AssemblyPaths.Length == 0)
                throw new InvalidOperationException("No assembly paths have been provided.");

#if FullNet
            var assemblies = PathUtilities.ExpandFileWildcards(AssemblyPaths)
                .Select(path => Assembly.LoadFrom(path)).ToArray();
#else
            var currentDirectory = await DynamicApis.DirectoryGetCurrentDirectoryAsync().ConfigureAwait(false);
            var assemblies = PathUtilities.ExpandFileWildcards(AssemblyPaths)
                .Select(path => assemblyLoader.Context.LoadFromAssemblyPath(PathUtilities.MakeAbsolutePath(path, currentDirectory))).ToArray();
#endif

            var allExportedNames = assemblies.SelectMany(a => a.ExportedTypes).Select(t => t.FullName).ToList();
            var matchedControllerNames = controllerNames
                .SelectMany(n => PathUtilities.FindWildcardMatches(n, allExportedNames, '.'))
                .Distinct();

            var controllerNamesWithoutWildcard = controllerNames.Where(n => !n.Contains("*")).ToArray();
            if (controllerNamesWithoutWildcard.Any(n => !matchedControllerNames.Contains(n)))
                throw new TypeLoadException("Unable to load type for controllers: " + string.Join(", ", controllerNamesWithoutWildcard));

            var controllerTypes = new List<Type>();
            foreach (var className in matchedControllerNames)
            {
                var controllerType = assemblies.Select(a => a.GetType(className)).FirstOrDefault(t => t != null);
                if (controllerType != null)
                    controllerTypes.Add(controllerType);
                else
                    throw new TypeLoadException("Unable to load type for controller: " + className);
            }

            return controllerTypes;
        }
    }
}
