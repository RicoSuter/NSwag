//-----------------------------------------------------------------------
// <copyright file="CodeGeneratorCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NConsole;
using Newtonsoft.Json;
using NJsonSchema.CodeGeneration;
using NSwag.CodeGeneration;

namespace NSwag.Commands.Base
{
    public abstract class CodeGeneratorCommand<TSettings> : InputOutputCommandBase
        where TSettings : ClientGeneratorBaseSettings
    {
        protected CodeGeneratorCommand(TSettings settings)
        {
            Settings = settings;
        }

        [JsonIgnore]
        public TSettings Settings { get; }

        [Argument(Name = "UseLiquidTemplates", IsRequired = false, Description = "Specifies whether to use Liquid templates (experimental).")]
        public bool UseLiquidTemplates
        {
            get { return Settings.CodeGeneratorSettings.UseLiquidTemplates; }
            set { Settings.CodeGeneratorSettings.UseLiquidTemplates = value; }
        }

        [Argument(Name = "TemplateDirectory", IsRequired = false, Description = "The Liquid template directory (experimental).")]
        public string TemplateDirectory
        {
            get { return Settings.CodeGeneratorSettings.TemplateDirectory; }
            set { Settings.CodeGeneratorSettings.TemplateDirectory = value; }
        }
    }
}