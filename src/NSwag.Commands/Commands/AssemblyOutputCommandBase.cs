using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NSwag.Commands.Base;
using NSwag.SwaggerGeneration;

namespace NSwag.Commands
{
    public abstract class AssemblyOutputCommandBase : OutputCommandBase
    {
        public AssemblyOutputCommandBase()
        {
            Settings = new AssemblyTypeToSwaggerGeneratorSettings();
        }

        [JsonIgnore]
        public AssemblyTypeToSwaggerGeneratorSettings Settings { get; set; }

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

        /// <summary>Creates a new generator instance.</summary>
        /// <returns>The generator.</returns>
        protected abstract Task<AssemblyTypeToSwaggerGeneratorBase> CreateGeneratorAsync();
    }
}