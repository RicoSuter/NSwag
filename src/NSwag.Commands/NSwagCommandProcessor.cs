//-----------------------------------------------------------------------
// <copyright file="NSwagCommandProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Reflection;
using NConsole;
using NJsonSchema;
using NJsonSchema.Infrastructure;
using NSwag.Commands.CodeGeneration;

namespace NSwag.Commands
{
    /// <summary></summary>
    public class NSwagCommandProcessor
    {
        private readonly IConsoleHost _host;

        /// <summary>Initializes a new instance of the <see cref="NSwagCommandProcessor" /> class.</summary>
        /// <param name="host">The host.</param>
        public NSwagCommandProcessor(IConsoleHost host)
        {
            _host = host;
        }

        /// <summary>Processes the command line arguments.</summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The result.</returns>
        public int Process(string[] args)
        {
            _host.WriteMessage("toolchain v" + OpenApiDocument.ToolchainVersion +
                " (NJsonSchema v" + JsonSchema.ToolchainVersion + ")\n");
            _host.WriteMessage("Visit http://NSwag.org for more information.\n");

            WriteBinDirectory();

            if (args.Length == 0)
            {
                _host.WriteMessage("Execute the 'help' command to show a list of all the available commands.\n");
            }

            try
            {
                var processor = new CommandLineProcessor(_host);

                processor.RegisterCommandsFromAssembly(typeof(SwaggerToCSharpControllerCommand).GetTypeInfo().Assembly);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                processor.Process(args);
                stopwatch.Stop();

                _host.WriteMessage("\nDuration: " + stopwatch.Elapsed + "\n");
            }
            catch (Exception exception)
            {
                _host.WriteError(exception.ToString());
                return -1;
            }

            WaitWhenDebuggerAttached();
            return 0;
        }

        private void WriteBinDirectory()
        {
            try
            {
                dynamic entryAssembly;
                var getEntryAssemblyMethod = typeof(Assembly).GetRuntimeMethod("GetEntryAssembly", new Type[] { });
                if (getEntryAssemblyMethod != null)
                {
                    entryAssembly = (Assembly)getEntryAssemblyMethod.Invoke(null, new object[] { });
                }
                else
                {
                    entryAssembly = typeof(NSwagCommandProcessor).GetTypeInfo().Assembly;
                }

                var binDirectory = DynamicApis.PathGetDirectoryName(new Uri(entryAssembly.CodeBase).LocalPath);
                _host.WriteMessage("NSwag bin directory: " + binDirectory + "\n");
            }
            catch (Exception exception)
            {
                _host.WriteMessage("NSwag bin directory could not be determined: " + exception.Message + "\n");
            }
        }

        private void WaitWhenDebuggerAttached()
        {
            if (Debugger.IsAttached)
            {
                _host.ReadValue("Press <enter> key to exit");
            }
        }
    }
}