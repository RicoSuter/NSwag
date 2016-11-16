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
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;
using NSwag.Commands.Base;

#pragma warning disable 1591

namespace NSwag.Commands
{
    [Command(Name = "webapi2swagger", Description = "Generates a Swagger specification for a controller or controlles contained in a .NET Web API assembly.")]
    public abstract class WebApiToSwaggerCommandBase : OutputCommandBase
    {
        public WebApiToSwaggerCommandBase()
        {
            Settings = new WebApiAssemblyToSwaggerGeneratorSettings();
            ControllerNames = new string[] { };
        }

        [JsonIgnore]
        public WebApiAssemblyToSwaggerGeneratorSettings Settings { get; set; }

        [Argument(Name = "Assembly", Description = "The path or paths to the Web API .NET assemblies (comma separated).")]
        public string[] AssemblyPaths
        {
            get { return Settings.AssemblyPaths; }
            set { Settings.AssemblyPaths = value; }
        }

        [Argument(Name = "AssemblyConfig", IsRequired = false, Description = "The path to the assembly App.config or Web.config (optional).")]
        public string AssemblyConfig
        {
            get { return Settings.AssemblyConfig; }
            set { Settings.AssemblyConfig = value; }
        }

        [Argument(Name = "ReferencePaths", IsRequired = false, Description = "The paths to search for referenced assembly files (comma separated).")]
        public string[] ReferencePaths
        {
            get { return Settings.ReferencePaths; }
            set { Settings.ReferencePaths = value; }
        }

        [Argument(Name = "AspNetCore", IsRequired = false, Description = "Specifies whether the controllers are hosted by ASP.NET Core.")]
        public bool IsAspNetCore
        {
            get { return Settings.IsAspNetCore; }
            set { Settings.IsAspNetCore = value; }
        }

        [JsonIgnore]
        [Argument(Name = "Controller", IsRequired = false, Description = "The Web API controller full class name or empty to load all controllers from the assembly.")]
        public string ControllerName
        {
            set { ControllerNames = new[] { value }; }
        }

        [Argument(Name = "Controllers", IsRequired = false, Description = "The Web API controller full class names or empty to load all controllers from the assembly (comma separated).")]
        public string[] ControllerNames { get; set; }

        [Argument(Name = "DefaultUrlTemplate", IsRequired = false, Description = "The Web API default URL template (default: 'api/{controller}/{id}').")]
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

        [Argument(Name = "AddMissingPathParameters", IsRequired = false, Description = "Specifies whether to add path parameters which are missing in the action method (default: false).")]
        public bool AddMissingPathParameters
        {
            get { return Settings.AddMissingPathParameters; }
            set { Settings.AddMissingPathParameters = value; }
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
            if (TryWriteFileOutput(host, () => service.ToJson()) == false)
                return service;
            return null;
        }

        public async Task<SwaggerDocument> RunAsync()
        {
            return await Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(DocumentTemplate))
                {
                    if (DynamicApis.FileExists(DocumentTemplate))
                        Settings.DocumentTemplate = DynamicApis.FileReadAllText(DocumentTemplate);
                    else
                        Settings.DocumentTemplate = DocumentTemplate;
                }
                else
                    Settings.DocumentTemplate = null;

                var generator = CreateGenerator();
                var controllerNames = ControllerNames.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                if (!controllerNames.Any() && Settings.AssemblyPaths?.Length > 0)
                    controllerNames = generator.GetControllerClasses().ToList();

                var document = generator.GenerateForControllers(controllerNames);

                if (ServiceHost == ".")
                    document.Host = string.Empty;
                else if (!string.IsNullOrEmpty(ServiceHost))
                    document.Host = ServiceHost;

                if (ServiceSchemes != null && ServiceSchemes.Any())
                    document.Schemes = ServiceSchemes.Select(s => (SwaggerSchema)Enum.Parse(typeof(SwaggerSchema), s, true)).ToList();

                if (!string.IsNullOrEmpty(ServiceBasePath))
                    document.BasePath = ServiceBasePath;

                return document;
            });
        }

        /// <summary>Creates a new generator instance.</summary>
        /// <returns>The generator.</returns>
        protected abstract WebApiAssemblyToSwaggerGeneratorBase CreateGenerator();
    }
}