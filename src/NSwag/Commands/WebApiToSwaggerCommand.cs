using System.Collections.Generic;
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
            ReferencePaths = new string[] { };
            ControllerNames = new string[] { };
        }

        [JsonIgnore]
        public WebApiAssemblyToSwaggerGeneratorSettings Settings { get; set; }

        [Description("The path to the Web API .NET assembly.")]
        [Argument(Name = "Assembly")]
        public string AssemblyPath
        {
            get { return Settings.AssemblyPath; }
            set { Settings.AssemblyPath = value; }
        }
        
        [Description("The path to seach for referenced assemblies files.")]
        [Argument(Name = "ReferencePaths", DefaultValue = new string[] { })]
        public string[] ReferencePaths
        {
            get { return Settings.ReferencePaths; }
            set { Settings.ReferencePaths = value; }
        }

        [Description("The Web API controller full class name or empty to load all controllers from the assembly.")]
        [Argument(Name = "Controller", DefaultValue = null)]
        public string ControllerName { get; set; }

        [Description("The Web API controller full class names or empty to load all controllers from the assembly.")]
        [Argument(Name = "Controllers", DefaultValue = new string[] { })]
        public string[] ControllerNames { get; set; }

        [Description("The Web API default URL template.")]
        [Argument(Name = "DefaultUrlTemplate", DefaultValue = "api/{controller}/{action}/{id}")]
        public string DefaultUrlTemplate
        {
            get { return Settings.DefaultUrlTemplate; }
            set { Settings.DefaultUrlTemplate = value; }
        }

        [Description("The default enum handling ('String' or 'Integer'), default: Integer.")]
        [Argument(Name = "DefaultEnumHandling", DefaultValue = EnumHandling.Integer)]
        public EnumHandling DefaultEnumHandling
        {
            get { return Settings.DefaultEnumHandling; }
            set { Settings.DefaultEnumHandling = value; }
        }

        [Description("Flatten the inheritance hierarchy instead of using allOf to describe inheritance (default: false).")]
        [Argument(Name = "FlattenInheritanceHierarchy", DefaultValue = false)]
        public bool FlattenInheritanceHierarchy
        {
            get { return Settings.FlattenInheritanceHierarchy; }
            set { Settings.FlattenInheritanceHierarchy = value; }
        }

        public override async Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            WriteOutput(host, await RunAsync());
        }

        public async Task<string> RunAsync()
        {
            var generator = new WebApiAssemblyToSwaggerGenerator(Settings);

            var controllerNames = ControllerNames.ToList();
            if (!string.IsNullOrEmpty(ControllerName))
                controllerNames.Add(ControllerName);

            controllerNames = controllerNames.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            if (!controllerNames.Any())
                controllerNames = generator.GetControllerClasses().ToList();

            var service = generator.GenerateForControllers(controllerNames);
            return service.ToJson();
        }
    }
}