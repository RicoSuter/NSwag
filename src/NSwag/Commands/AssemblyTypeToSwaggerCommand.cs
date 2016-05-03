using System.ComponentModel;
using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag.CodeGeneration.SwaggerGenerators;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;
using NSwag.Commands.Base;

namespace NSwag.Commands
{
    public class AssemblyTypeToSwaggerCommand : OutputCommandBase
    {
        public AssemblyTypeToSwaggerCommand()
        {
            Settings = new AssemblyTypeToSwaggerGeneratorSettings();
            ClassNames = new string[] { };
        }

        [JsonIgnore]
        public AssemblyTypeToSwaggerGeneratorSettings Settings { get; set; }

        [Description("The path to the Web API .NET assembly.")]
        [Argument(Name = "Assembly")]
        public string AssemblyPath
        {
            get { return Settings.AssemblyPath; }
            set { Settings.AssemblyPath = value; }
        }

        [Description("The path to the assembly App.config or Web.config (optional).")]
        [Argument(Name = "AssemblyConfig", DefaultValue = "")]
        public string AssemblyConfig
        {
            get { return Settings.AssemblyConfig; }
            set { Settings.AssemblyConfig = value; }
        }

        [Description("The paths to search for referenced assembly files.")]
        [Argument(Name = "ReferencePaths", DefaultValue = new string[] { })]
        public string[] ReferencePaths
        {
            get { return Settings.ReferencePaths; }
            set { Settings.ReferencePaths = value; }
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

        [Description("Generate schemas for types in KnownTypeAttribute attributes (default: true).")]
        [Argument(Name = "GenerateKnownTypes", DefaultValue = true)]
        public bool GenerateKnownTypes
        {
            get { return Settings.GenerateKnownTypes; }
            set { Settings.GenerateKnownTypes = value; }
        }

        [Description("The class names.")]
        [Argument(Name = "ClassNames", DefaultValue = null)]
        public string[] ClassNames { get; set; }

        public override async Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            WriteOutput(host, await RunAsync());
        }

        public async Task<string> RunAsync()
        {
            var generator = new AssemblyTypeToSwaggerGenerator(Settings);
            var service = generator.Generate(ClassNames);
            return service.ToJson();
        }
    }
}