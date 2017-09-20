//-----------------------------------------------------------------------
// <copyright file="WebApiToSwaggerCommandBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Infrastructure;
using NSwag.SwaggerGeneration;
using NSwag.SwaggerGeneration.WebApi;

#pragma warning disable 1591

namespace NSwag.Commands
{
    [Command(Name = "webapi2swagger", Description = "Generates a Swagger specification for a controller or controlles contained in a .NET Web API assembly.")]
    public abstract class WebApiToSwaggerCommandBase : AssemblyOutputCommandBase<WebApiAssemblyToSwaggerGeneratorBase>
    {
        protected WebApiToSwaggerCommandBase(IAssemblySettings settings)
            : base(settings)
        {
            ControllerNames = new string[] { };
        }

        public new WebApiAssemblyToSwaggerGeneratorSettings Settings => (WebApiAssemblyToSwaggerGeneratorSettings)base.Settings;

        [Argument(Name = "Assembly", Description = "The path or paths to the Web API .NET assemblies (comma separated).")]
        public string[] AssemblyPaths
        {
            get { return Settings.AssemblySettings.AssemblyPaths; }
            set { Settings.AssemblySettings.AssemblyPaths = value; }
        }

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

        [Argument(Name = "GenerateAbstractProperties", IsRequired = false, Description = "Generate abstract properties (i.e. interface and abstract properties. Properties may defined multiple times in a inheritance hierarchy, default: false).")]
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

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var service = await RunAsync();
            await TryWriteFileOutputAsync(host, () => service.ToJson()).ConfigureAwait(false);
            return service;
        }

        public async Task<SwaggerDocument> RunAsync()
        {
            return await Task.Run(async () =>
            {
                if (!string.IsNullOrEmpty(DocumentTemplate))
                {
                    if (await DynamicApis.FileExistsAsync(DocumentTemplate).ConfigureAwait(false))
                        Settings.DocumentTemplate = await DynamicApis.FileReadAllTextAsync(DocumentTemplate).ConfigureAwait(false);
                    else
                        Settings.DocumentTemplate = DocumentTemplate;
                }
                else
                    Settings.DocumentTemplate = null;

                var generator = await CreateGeneratorAsync();

                var controllerNames = ControllerNames.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                if (!controllerNames.Any() && Settings.AssemblySettings.AssemblyPaths?.Length > 0)
                    controllerNames = generator.GetExportedControllerClassNames().ToList();

                var document = await generator.GenerateForControllersAsync(controllerNames).ConfigureAwait(false);
                if (ServiceHost == ".")
                    document.Host = string.Empty;
                else if (!string.IsNullOrEmpty(ServiceHost))
                    document.Host = ServiceHost;

                if (!string.IsNullOrEmpty(InfoTitle))
                    document.Info.Title = InfoTitle;
                if (!string.IsNullOrEmpty(InfoVersion))
                    document.Info.Version = InfoVersion;
                if (!string.IsNullOrEmpty(InfoDescription))
                    document.Info.Description = InfoDescription;

                if (ServiceSchemes != null && ServiceSchemes.Any())
                    document.Schemes = ServiceSchemes.Select(s => (SwaggerSchema)Enum.Parse(typeof(SwaggerSchema), s, true)).ToList();

                if (!string.IsNullOrEmpty(ServiceBasePath))
                    document.BasePath = ServiceBasePath;

                return document;
            });
        }
    }
}