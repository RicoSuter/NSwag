//-----------------------------------------------------------------------
// <copyright file="NSwagCommandProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NConsole;
using NJsonSchema;

namespace NSwag.Commands
{
    /// <summary></summary>
    public class NSwagCommandProcessor
    {
        private readonly Assembly _assemblyLoaderAssembly;
        private readonly IConsoleHost _host;

        /// <summary>Initializes a new instance of the <see cref="NSwagCommandProcessor" /> class.</summary>
        /// <param name="assemblyLoaderAssembly">The command assembly.</param>
        /// <param name="host">The host.</param>
        public NSwagCommandProcessor(Assembly assemblyLoaderAssembly, IConsoleHost host)
        {
            _assemblyLoaderAssembly = assemblyLoaderAssembly;
            _host = host;
        }

        /// <summary>Processes the command line arguments.</summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The result.</returns>
        public int Process(string[] args)
        {
            _host.WriteMessage("NSwag command line: NSwag toolchain v" + SwaggerService.ToolchainVersion +
                              " (NJsonSchema v" + JsonSchema4.ToolchainVersion + ")" +
                              (IntPtr.Size == 4 ? " (x86)" : " (x64)") + "\n");

            _host.WriteMessage("Visit http://NSwag.org for more information.\n");

            if (args.Length == 0)
                _host.WriteMessage("Execute the 'help' command to show a list of all the available commands.\n");

            try
            {
                var processor = new CommandLineProcessor(_host);

                processor.RegisterCommandsFromAssembly(_assemblyLoaderAssembly);
                processor.RegisterCommandsFromAssembly(typeof(SwaggerToCSharpControllerCommand).GetTypeInfo().Assembly);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var results = processor.Process(args);
                stopwatch.Stop();

                var output = results.Last()?.Output;
                var service = output as SwaggerService;
                if (service != null)
                    _host.WriteMessage(service.ToJson());
                else if (output != null)
                    _host.WriteMessage(output.ToString());

                _host.WriteMessage("\nDuration: " + stopwatch.Elapsed);
            }
            catch (Exception exception)
            {
                _host.WriteError(exception.ToString());
                return -1;
            }

            WaitWhenDebuggerAttached();
            return 0;
        }

        private void WaitWhenDebuggerAttached()
        {
            if (Debugger.IsAttached)
                _host.ReadValue("Press <enter> key to exit");
        }
    }
}