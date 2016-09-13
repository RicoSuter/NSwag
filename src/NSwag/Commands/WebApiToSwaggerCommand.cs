using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;
using NSwag.Commands.Base;

namespace NSwag.Commands
{
    [Description("Generates a Swagger specification for a controller or controlles contained in a .NET Web API assembly.")]
    public class WebApiToSwaggerCommand : OutputCommandBase
    {
        public WebApiToSwaggerCommand()
        {
            Settings = new WebApiAssemblyToSwaggerGeneratorSettings();
            ControllerNames = new string[] { };
        }

        [JsonIgnore]
        public WebApiAssemblyToSwaggerGeneratorSettings Settings { get; set; }

        public string AssemblyPath
        {
            get { return Settings.AssemblyPaths.FirstOrDefault(); }
            set { Settings.AssemblyPaths = !string.IsNullOrEmpty(value) ? new[] { value } : new string[] { }; }
        }

        [Description("The path or paths to the Web API .NET assemblies (comma separated).")]
        [Argument(Name = "Assembly")]
        public string[] AssemblyPaths
        {
            get { return Settings.AssemblyPaths; }
            set { Settings.AssemblyPaths = value; }
        }

        [Description("The path to the assembly App.config or Web.config (optional).")]
        [Argument(Name = "AssemblyConfig", IsRequired = false)]
        public string AssemblyConfig
        {
            get { return Settings.AssemblyConfig; }
            set { Settings.AssemblyConfig = value; }
        }

        [Description("The paths to search for referenced assembly files (comma separated).")]
        [Argument(Name = "ReferencePaths", IsRequired = false)]
        public string[] ReferencePaths
        {
            get { return Settings.ReferencePaths; }
            set { Settings.ReferencePaths = value; }
        }

        [Description("The Web API controller full class name or empty to load all controllers from the assembly.")]
        [Argument(Name = "Controller", IsRequired = false)]
        public string ControllerName { get; set; }

        [Description("The Web API controller full class names or empty to load all controllers from the assembly (comma separated).")]
        [Argument(Name = "Controllers", IsRequired = false)]
        public string[] ControllerNames { get; set; }

        [Description("The Web API default URL template (default: 'api/{controller}/{id}').")]
        [Argument(Name = "DefaultUrlTemplate", IsRequired = false)]
        public string DefaultUrlTemplate
        {
            get { return Settings.DefaultUrlTemplate; }
            set { Settings.DefaultUrlTemplate = value; }
        }

        [Description("The default property name handling ('Default' or 'CamelCase').")]
        [Argument(Name = "DefaultPropertyNameHandling", IsRequired = false)]
        public PropertyNameHandling DefaultPropertyNameHandling
        {
            get { return Settings.DefaultPropertyNameHandling; }
            set { Settings.DefaultPropertyNameHandling = value; }
        }

        [Description("The default enum handling ('String' or 'Integer'), default: Integer.")]
        [Argument(Name = "DefaultEnumHandling", IsRequired = false)]
        public EnumHandling DefaultEnumHandling
        {
            get { return Settings.DefaultEnumHandling; }
            set { Settings.DefaultEnumHandling = value; }
        }

        [Description("Flatten the inheritance hierarchy instead of using allOf to describe inheritance (default: false).")]
        [Argument(Name = "FlattenInheritanceHierarchy", IsRequired = false)]
        public bool FlattenInheritanceHierarchy
        {
            get { return Settings.FlattenInheritanceHierarchy; }
            set { Settings.FlattenInheritanceHierarchy = value; }
        }

        [Description("Generate schemas for types in KnownTypeAttribute attributes (default: true).")]
        [Argument(Name = "GenerateKnownTypes", IsRequired = false)]
        public bool GenerateKnownTypes
        {
            get { return Settings.GenerateKnownTypes; }
            set { Settings.GenerateKnownTypes = value; }
        }


        [Description("Overrides the service host of the web service (optional).")]
        [Argument(Name = "ServiceHost", IsRequired = false)]
        public string ServiceHost { get; set; }

        [Description("Overrides the allowed schemes of the web service (optional, comma separated, 'http', 'https', 'ws', 'wss').")]
        [Argument(Name = "ServiceSchemes", IsRequired = false)]
        public string[] ServiceSchemes { get; set; }


        [Description("Specify the title of the Swagger specification.")]
        [Argument(Name = "InfoTitle", IsRequired = false)]
        public string InfoTitle
        {
            get { return Settings.Title; }
            set { Settings.Title = value; }
        }

        [Description("Specify the description of the Swagger specification.")]
        [Argument(Name = "InfoDescription", IsRequired = false)]
        public string InfoDescription
        {
            get { return Settings.Description; }
            set { Settings.Description = value; }
        }

        [Description("Specify the version of the Swagger specification (default: 1.0.0).")]
        [Argument(Name = "InfoVersion", IsRequired = false)]
        public string InfoVersion
        {
            get { return Settings.Version; }
            set { Settings.Version = value; }
        }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var service = await RunAsync();
            if (TryWriteFileOutput(host, () => service.ToJson()) == false)
                return service;
            return null;
        }

        public async Task<SwaggerService> RunAsync()
        {
            return await Task.Run(() =>
            {
                var generator = new WebApiAssemblyToSwaggerGenerator(Settings);

                var controllerNames = ControllerNames.ToList();
                if (!string.IsNullOrEmpty(ControllerName))
                    controllerNames.Add(ControllerName);

                controllerNames = controllerNames.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                if (!controllerNames.Any() && Settings.AssemblyPaths?.Length > 0)
                    controllerNames = generator.GetControllerClasses().ToList();

                var service = generator.GenerateForControllers(controllerNames);

                if (!string.IsNullOrEmpty(ServiceHost))
                    service.Host = ServiceHost;
                if (ServiceSchemes != null && ServiceSchemes.Any())
                    service.Schemes = ServiceSchemes.Select(s => (SwaggerSchema)Enum.Parse(typeof(SwaggerSchema), s, true)).ToList();

                return service;
            });
        }
    }
}