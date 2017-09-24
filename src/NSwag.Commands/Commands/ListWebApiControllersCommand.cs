//-----------------------------------------------------------------------
// <copyright file="ListWebApiControllersCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using NConsole;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.Commands
{
    [Command(Name = "list-controllers", Description = "List all controllers classes for the given assembly and settings.")]
    public class ListWebApiControllersCommand : AssemblyOutputCommandBase<WebApiAssemblyToSwaggerGenerator>
    {
        public ListWebApiControllersCommand()
            : base(new WebApiAssemblyToSwaggerGeneratorSettings())
        {
        }

        [Argument(Name = "File", IsRequired = false, Description = "The nswag.json configuration file path.")]
        public string File { get; set; }

        [Argument(Name = "Assembly", IsRequired = false, Description = "The path or paths to the Web API .NET assemblies (comma separated).")]
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
                var controllers = generator.GetExportedControllerClassNames();

                host.WriteMessage("\r\n");
                foreach (var controller in controllers)
                    host.WriteMessage(controller + "\r\n");
                host.WriteMessage("\r\n");

                return controllers;
            });
        }

        /// <summary>Creates a new generator instance.</summary>
        /// <returns>The generator.</returns>
        /// <exception cref="InvalidOperationException">Configuraiton file does not contain WebApiToSwagger settings.</exception>
        protected override async Task<WebApiAssemblyToSwaggerGenerator> CreateGeneratorAsync()
        {
            if (!string.IsNullOrEmpty(File))
            {
                var document = await NSwagDocument.LoadAsync(File);

                var settings = document.SwaggerGenerators?.WebApiToSwaggerCommand?.Settings;
                if (settings == null)
                    throw new InvalidOperationException("Configuraiton file does not contain WebApiToSwagger settings.");

                return new WebApiAssemblyToSwaggerGenerator(settings);
            }
            else
                return new WebApiAssemblyToSwaggerGenerator((WebApiAssemblyToSwaggerGeneratorSettings)Settings);
        }
    }
}