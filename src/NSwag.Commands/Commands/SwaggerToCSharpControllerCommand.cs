//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpControllerCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using NConsole;
using NSwag.CodeGeneration.CodeGenerators.CSharp;

#pragma warning disable 1591

namespace NSwag.Commands
{
    [Command(Name = "swagger2cscontroller", Description = "Generates CSharp Web API controller code from a Swagger specification.")]
    public class SwaggerToCSharpControllerCommand : SwaggerToCSharpCommand<SwaggerToCSharpWebApiControllerGeneratorSettings>
    {
        public SwaggerToCSharpControllerCommand() : base(new SwaggerToCSharpWebApiControllerGeneratorSettings())
        {
        }

        [Argument(Name = "ControllerBaseClass", Description = "The controller base class (empty for 'ApiController').", IsRequired = false)]
        public string ControllerBaseClass
        {
            get { return Settings.ControllerBaseClass; }
            set { Settings.ControllerBaseClass = value; }
        }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var code = await RunAsync();
            if (await TryWriteFileOutputAsync(host, () => code).ConfigureAwait(false) == false)
                return code;
            return null;
        }

        public async Task<string> RunAsync()
        {
            return await Task.Run(async () =>
            {
                var document = await GetInputSwaggerDocument().ConfigureAwait(false);
                var clientGenerator = new SwaggerToCSharpWebApiControllerGenerator(document, Settings);
                return clientGenerator.GenerateFile();
            });
        }
    }
}