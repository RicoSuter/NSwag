//-----------------------------------------------------------------------
// <copyright file="ListTypesCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using NConsole;
using NSwag.SwaggerGenerators;

namespace NSwag.Commands
{
    [Command(Name = "list-types", Description = "List all types for the given assembly and settings.")]
    public class ListTypesCommand : AssemblyOutputCommandBase<AssemblyTypeToSwaggerGenerator>
    {
        public ListTypesCommand() 
            : base(new AssemblyTypeToSwaggerGeneratorSettings())
        {
        }

        [Argument(Name = "File", IsRequired = false, Description = "The nswag.json configuration file path.")]
        public string File { get; set; }

        [Argument(Name = "Assembly", IsRequired = false, Description = "The path to the Web API .NET assembly.")]
        public string[] AssemblyPaths
        {
            get { return Settings.AssemblySettings.AssemblyPaths; }
            set { Settings.AssemblySettings.AssemblyPaths = value; }
        }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            return await Task.Run(async () =>
            {
                var generator = await CreateGeneratorAsync();
                var classNames = generator.GetExportedClassNames();

                host.WriteMessage("\r\n");
                foreach (var className in classNames)
                    host.WriteMessage(className + "\r\n");
                host.WriteMessage("\r\n");

                return classNames;
            });
        }

        protected override async Task<AssemblyTypeToSwaggerGenerator> CreateGeneratorAsync()
        {
            if (!string.IsNullOrEmpty(File))
            {
                var document = await NSwagDocument.LoadAsync(File);

                var settings = document.SwaggerGenerators?.AssemblyTypeToSwaggerCommand?.Settings;
                if (settings == null)
                    throw new InvalidOperationException("Configuraiton file does not contain AssemblyTypeToSwagger settings.");

                return new AssemblyTypeToSwaggerGenerator(settings);
            }
            else
                return new AssemblyTypeToSwaggerGenerator((AssemblyTypeToSwaggerGeneratorSettings)Settings);
        }
    }
}