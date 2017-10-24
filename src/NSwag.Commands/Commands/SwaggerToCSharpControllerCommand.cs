//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpControllerCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using NConsole;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.CSharp.Models;

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
        
        [Argument(Name = "ControllerStyle", Description = "The controller generation style (partial, abstract; default: partial).", IsRequired = false)]
        public CSharpControllerStyle ControllerStyle
        {
            get { return Settings.ControllerStyle; }
            set { Settings.ControllerStyle = value; }
        }

        [Argument(Name = "UseCancellationToken", Description = "Add a cancellation token parameter (default: false).", IsRequired = false)]
        public bool UseCancellationToken
        {
            get { return Settings.UseCancellationToken; }
            set { Settings.UseCancellationToken = value; }
        }

        [Argument(Name = "AspNetNamespace", Description = "The ASP.NET (Core) framework namespace (default: 'System.Web.Http').", IsRequired = false)]
        public string AspNetNamespace
        {
            get { return Settings.AspNetNamespace; }
            set { Settings.AspNetNamespace = value; }
        }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var code = await RunAsync();
            await TryWriteFileOutputAsync(host, () => code).ConfigureAwait(false);
            return code;
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
