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

        /// <summary>Initializes a new instance of the <see cref="NSwagCommandProcessor"/> class.</summary>
        /// <param name="assemblyLoaderAssembly">The command assembly.</param>
        public NSwagCommandProcessor(Assembly assemblyLoaderAssembly)
        {
            _assemblyLoaderAssembly = assemblyLoaderAssembly;
        }

        /// <summary>Processes the command line arguments.</summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The result.</returns>
        public int Process(string[] args)
        {
            var host = new ConsoleHost();
            host.WriteMessage("NSwag command line: NSwag toolchain v" + SwaggerService.ToolchainVersion +
                              " (NJsonSchema v" + JsonSchema4.ToolchainVersion + ")" +
                              (IntPtr.Size == 4 ? " (x86)" : " (x64)") + "\n");

            host.WriteMessage("Visit http://NSwag.org for more information.\n");

            if (args.Length == 0)
                host.WriteMessage("Execute the 'help' command to show a list of all the available commands.\n");

            try
            {
                var processor = new CommandLineProcessor(host);

                processor.RegisterCommandsFromAssembly(_assemblyLoaderAssembly);
                processor.RegisterCommandsFromAssembly(typeof(SwaggerToCSharpControllerCommand).GetTypeInfo().Assembly);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var results = processor.Process(args);
                stopwatch.Stop();

                var output = results.Last()?.Output;
                var service = output as SwaggerService;
                if (service != null)
                    host.WriteMessage(service.ToJson());
                else if (output != null)
                    host.WriteMessage(output.ToString());

                host.WriteMessage("\nDuration: " + stopwatch.Elapsed);
            }
            catch (Exception exception)
            {
                host.WriteError(exception.ToString());
                WaitWhenDebuggerAttached(host);
                return -1;
            }

            WaitWhenDebuggerAttached(host);
            return 0;
        }

        private void WaitWhenDebuggerAttached(ConsoleHost host)
        {
            if (Debugger.IsAttached)
                host.ReadValue("Press <enter> key to exit");
        }
    }
}