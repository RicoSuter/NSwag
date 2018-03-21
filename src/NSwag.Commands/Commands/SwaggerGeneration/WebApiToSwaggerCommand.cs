//-----------------------------------------------------------------------
// <copyright file="NSwagSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Infrastructure;
using NSwag.AssemblyLoader.Utilities;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.Commands.SwaggerGeneration
{
    /// <summary>The generator.</summary>
    [Command(Name = "webapi2swagger", Description = "Generates a Swagger specification for a controller or controlles contained in a .NET Web API assembly.")]
    public class WebApiToSwaggerCommand : IsolatedSwaggerOutputCommandBase
    {
        /// <summary>Initializes a new instance of the <see cref="WebApiToSwaggerCommand"/> class.</summary>
        public WebApiToSwaggerCommand()
        {
            Settings = new WebApiToSwaggerGeneratorSettings();
            ControllerNames = new string[] { };
        }

        [JsonIgnore]
        public WebApiToSwaggerGeneratorSettings Settings { get; }

        [JsonIgnore]
        [Argument(Name = "Controller", IsRequired = false, Description = "The Web API controller full class name or empty to load all controllers from the assembly.")]
        public string ControllerName
        {
            get { return ControllerNames.FirstOrDefault(); }
            set { ControllerNames = new[] { value }; }
        }

        [Argument(Name = "Controllers", IsRequired = false, Description = "The Web API controller full class names or empty to load all controllers from the assembly (comma separated).")]
        public string[] ControllerNames { get; set; }

        [Argument(Name = "AspNetCore", IsRequired = false, Description = "Specifies whether the controllers are hosted by ASP.NET Core.")]
        public bool IsAspNetCore
        {
            get { return Settings.IsAspNetCore; }
            set { Settings.IsAspNetCore = value; }
        }

        [Argument(Name = "DefaultUrlTemplate", IsRequired = false, Description = "The Web API default URL template (default for Web API: 'api/{controller}/{id}'; for MVC projects: '{controller}/{action}/{id?}').")]
        public string DefaultUrlTemplate
        {
            get { return Settings.DefaultUrlTemplate; }
            set { Settings.DefaultUrlTemplate = value; }
        }

        [Argument(Name = "DefaultPropertyNameHandling", IsRequired = false, Description = "The default property name handling ('Default' or 'CamelCase').")]
        public PropertyNameHandling DefaultPropertyNameHandling
        {
            get { return Settings.DefaultPropertyNameHandling; }
            set { Settings.DefaultPropertyNameHandling = value; }
        }

        [Argument(Name = "DefaultReferenceTypeNullHandling", IsRequired = false, Description = "The default null handling (if NotNullAttribute and CanBeNullAttribute are missing, default: Null, Null or NotNull).")]
        public ReferenceTypeNullHandling DefaultReferenceTypeNullHandling
        {
            get { return Settings.DefaultReferenceTypeNullHandling; }
            set { Settings.DefaultReferenceTypeNullHandling = value; }
        }

        [Argument(Name = "DefaultEnumHandling", IsRequired = false, Description = "The default enum handling ('String' or 'Integer'), default: Integer.")]
        public EnumHandling DefaultEnumHandling
        {
            get { return Settings.DefaultEnumHandling; }
            set { Settings.DefaultEnumHandling = value; }
        }

        [Argument(Name = "FlattenInheritanceHierarchy", IsRequired = false, Description = "Flatten the inheritance hierarchy instead of using allOf to describe inheritance (default: false).")]
        public bool FlattenInheritanceHierarchy
        {
            get { return Settings.FlattenInheritanceHierarchy; }
            set { Settings.FlattenInheritanceHierarchy = value; }
        }

        [Argument(Name = "GenerateKnownTypes", IsRequired = false, Description = "Generate schemas for types in KnownTypeAttribute attributes (default: true).")]
        public bool GenerateKnownTypes
        {
            get { return Settings.GenerateKnownTypes; }
            set { Settings.GenerateKnownTypes = value; }
        }

        [Argument(Name = "GenerateXmlObjects", IsRequired = false, Description = "Generate xmlObject representation for definitions (default: false).")]
        public bool GenerateXmlObjects
        {
            get { return Settings.GenerateXmlObjects; }
            set { Settings.GenerateXmlObjects = value; }
        }

        [Argument(Name = "GenerateAbstractProperties", IsRequired = false, Description = "Generate abstract properties (i.e. interface and abstract properties. " +
            "Properties may defined multiple times in a inheritance hierarchy, default: false).")]
        public bool GenerateAbstractProperties
        {
            get { return Settings.GenerateAbstractProperties; }
            set { Settings.GenerateAbstractProperties = value; }
        }

        [Argument(Name = "AddMissingPathParameters", IsRequired = false, Description = "Specifies whether to add path parameters which are missing in the action method (default: true).")]
        public bool AddMissingPathParameters
        {
            get { return Settings.AddMissingPathParameters; }
            set { Settings.AddMissingPathParameters = value; }
        }

        [Argument(Name = "IgnoreObsoleteProperties", IsRequired = false, Description = "Ignore properties with the ObsoleteAttribute (default: false).")]
        public bool IgnoreObsoleteProperties
        {
            get { return Settings.IgnoreObsoleteProperties; }
            set { Settings.IgnoreObsoleteProperties = value; }
        }

        [Argument(Name = "AllowReferencesWithProperties", IsRequired = false, Description = "Use $ref references even if additional properties are defined on " +
            "the object (otherwise allOf/oneOf with $ref is used, default: false).")]
        public bool AllowReferencesWithProperties
        {
            get { return Settings.AllowReferencesWithProperties; }
            set { Settings.AllowReferencesWithProperties = value; }
        }

        [Argument(Name = "ExcludedTypeNames", IsRequired = false, Description = "The excluded type names (same as JsonSchemaIgnoreAttribute).")]
        public string[] ExcludedTypeNames
        {
            get { return Settings.ExcludedTypeNames; }
            set { Settings.ExcludedTypeNames = value; }
        }

        [Argument(Name = "ServiceHost", IsRequired = false, Description = "Overrides the service host of the web service (optional, use '.' to remove the hostname).")]
        public string ServiceHost { get; set; }

        [Argument(Name = "ServiceBasePath", IsRequired = false, Description = "The basePath of the Swagger specification (optional).")]
        public string ServiceBasePath { get; set; }

        [Argument(Name = "ServiceSchemes", IsRequired = false, Description = "Overrides the allowed schemes of the web service (optional, comma separated, 'http', 'https', 'ws', 'wss').")]
        public string[] ServiceSchemes { get; set; }

        [Argument(Name = "InfoTitle", IsRequired = false, Description = "Specify the title of the Swagger specification.")]
        public string InfoTitle
        {
            get { return Settings.Title; }
            set { Settings.Title = value; }
        }

        [Argument(Name = "InfoDescription", IsRequired = false, Description = "Specify the description of the Swagger specification.")]
        public string InfoDescription
        {
            get { return Settings.Description; }
            set { Settings.Description = value; }
        }

        [Argument(Name = "InfoVersion", IsRequired = false, Description = "Specify the version of the Swagger specification (default: 1.0.0).")]
        public string InfoVersion
        {
            get { return Settings.Version; }
            set { Settings.Version = value; }
        }

        [Argument(Name = "DocumentTemplate", IsRequired = false, Description = "Specifies the Swagger document template (may be a path or JSON, default: none).")]
        public string DocumentTemplate { get; set; }

        [Argument(Name = "DocumentProcessors", IsRequired = false, Description = "The document processor type names in the form 'assemblyName:fullTypeName' or 'fullTypeName').")]
        public string[] DocumentProcessorTypes { get; set; }

        [Argument(Name = "OperationProcessors", IsRequired = false, Description = "The operation processor type names in the form 'assemblyName:fullTypeName' or 'fullTypeName').")]
        public string[] OperationProcessorTypes { get; set; }

        [Argument(Name = "TypeNameGenerator", IsRequired = false, Description = "The custom ITypeNameGenerator implementation type in the form 'assemblyName:fullTypeName' or 'fullTypeName').")]
        public string TypeNameGeneratorType { get; set; }

        [Argument(Name = "SchemaNameGenerator", IsRequired = false, Description = "The custom ISchemaNameGenerator implementation type in the form 'assemblyName:fullTypeName' or 'fullTypeName').")]
        public string SchemaNameGeneratorType { get; set; }

        protected override async Task<string> RunIsolatedAsync(AssemblyLoader.AssemblyLoader assemblyLoader)
        {
            await TransformAsync(assemblyLoader);

            var controllerNames = ControllerNames.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            if (!controllerNames.Any() && AssemblyPaths?.Length > 0)
                controllerNames = GetControllerNames(assemblyLoader).ToList();

            var controllerTypes = await GetControllerTypesAsync(controllerNames, assemblyLoader);

            var generator = new WebApiToSwaggerGenerator(Settings);
            var document = await generator.GenerateForControllersAsync(controllerTypes).ConfigureAwait(false);

            if (ServiceHost == ".")
                document.Host = string.Empty;
            else if (!string.IsNullOrEmpty(ServiceHost))
                document.Host = ServiceHost;

            if (string.IsNullOrEmpty(DocumentTemplate))
            {
                if (!string.IsNullOrEmpty(InfoTitle))
                    document.Info.Title = InfoTitle;
                if (!string.IsNullOrEmpty(InfoVersion))
                    document.Info.Version = InfoVersion;
                if (!string.IsNullOrEmpty(InfoDescription))
                    document.Info.Description = InfoDescription;
            }

            if (ServiceSchemes != null && ServiceSchemes.Any())
                document.Schemes = ServiceSchemes.Select(s => (SwaggerSchema)Enum.Parse(typeof(SwaggerSchema), s, true)).ToList();

            if (!string.IsNullOrEmpty(ServiceBasePath))
                document.BasePath = ServiceBasePath;

            return document.ToJson();
        }

        private async Task TransformAsync(AssemblyLoader.AssemblyLoader assemblyLoader)
        {
            if (!string.IsNullOrEmpty(DocumentTemplate))
            {
                if (await DynamicApis.FileExistsAsync(DocumentTemplate).ConfigureAwait(false))
                    Settings.DocumentTemplate = await DynamicApis.FileReadAllTextAsync(DocumentTemplate).ConfigureAwait(false);
                else
                    Settings.DocumentTemplate = DocumentTemplate;

                if (!string.IsNullOrEmpty(Settings.DocumentTemplate) && !Settings.DocumentTemplate.StartsWith("{"))
                    Settings.DocumentTemplate = (await SwaggerYamlDocument.FromYamlAsync(Settings.DocumentTemplate)).ToJson();
            }
            else
                Settings.DocumentTemplate = null;

            if (DocumentProcessorTypes != null)
            {
                foreach (var p in DocumentProcessorTypes)
                {
                    var processor = (IDocumentProcessor)assemblyLoader.CreateInstance(p);
                    Settings.DocumentProcessors.Add(processor);
                }
            }

            if (OperationProcessorTypes != null)
            {
                foreach (var p in OperationProcessorTypes)
                {
                    var processor = (IOperationProcessor)assemblyLoader.CreateInstance(p);
                    Settings.OperationProcessors.Add(processor);
                }
            }

            if (!string.IsNullOrEmpty(TypeNameGeneratorType))
                Settings.TypeNameGenerator = (ITypeNameGenerator)assemblyLoader.CreateInstance(TypeNameGeneratorType);

            if (!string.IsNullOrEmpty(SchemaNameGeneratorType))
                Settings.SchemaNameGenerator = (ISchemaNameGenerator)assemblyLoader.CreateInstance(SchemaNameGeneratorType);
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
