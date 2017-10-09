//-----------------------------------------------------------------------
// <copyright file="AssemblyOutputCommandBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NSwag.Commands.Base;
using NSwag.SwaggerGeneration;

namespace NSwag.Commands
{
    /// <summary>A command with assembly settings.</summary>
    /// <typeparam name="TGenerator"></typeparam>
    public abstract class AssemblyOutputCommandBase<TGenerator> : OutputCommandBase
    {
        /// <summary>Initializes a new instance of the <see cref="AssemblyOutputCommandBase{TGenerator}"/> class.</summary>
        /// <param name="settings">The settings.</param>
        public AssemblyOutputCommandBase(IAssemblySettings settings)
        {
            Settings = settings;
        }

        [JsonIgnore]
        public IAssemblySettings Settings { get; }

        [Argument(Name = "AssemblyConfig", IsRequired = false, Description = "The path to the assembly App.config or Web.config (optional).")]
        public string AssemblyConfig
        {
            get { return Settings.AssemblySettings.AssemblyConfig; }
            set { Settings.AssemblySettings.AssemblyConfig = value; }
        }

        [Argument(Name = "ReferencePaths", IsRequired = false, Description = "The paths to search for referenced assembly files (comma separated).")]
        public string[] ReferencePaths
        {
            get { return Settings.AssemblySettings.ReferencePaths; }
            set { Settings.AssemblySettings.ReferencePaths = value; }
        }
    }
}