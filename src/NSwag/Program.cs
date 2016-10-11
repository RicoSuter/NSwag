using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NConsole;
using NJsonSchema;
using NSwag.Commands;

namespace NSwag
{
    public class Program
    {
        static int Main(string[] args)
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

                processor.RegisterCommand<NSwagDocumentCommand>("run");

                processor.RegisterCommand<AssemblyTypeToSwaggerCommand>("types2swagger");
                processor.RegisterCommand<WebApiToSwaggerCommand>("webapi2swagger");

                processor.RegisterCommand<JsonSchemaToCSharpCommand>("jsonschema2csclient");
                processor.RegisterCommand<JsonSchemaToTypeScriptCommand>("jsonschema2tsclient");

                processor.RegisterCommand<SwaggerToCSharpClientCommand>("swagger2csclient");
                processor.RegisterCommand<SwaggerToCSharpControllerCommand>("swagger2cscontroller");
                processor.RegisterCommand<SwaggerToTypeScriptClientCommand>("swagger2tsclient");

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
                var savedForegroundColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                host.WriteMessage(exception.ToString());
                Console.ForegroundColor = savedForegroundColor;

                WaitWhenDebuggerAttached();
                return -1;
            }

            WaitWhenDebuggerAttached();
            return 0;
        }

        private static void WaitWhenDebuggerAttached()
        {
            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press <any> key to exit...");
                Console.ReadKey();
            }
        }

        private static string GetVersionWithBuildTime()
        {
            var assembly = typeof(SwaggerInfo).Assembly;
            return assembly.GetName().Version + " (" + GetBuildTime(assembly) + ")";
        }

        private static DateTime GetBuildTime(Assembly assembly)
        {
            Version version = assembly.GetName().Version;
            DateTime dateTime = new DateTime(2000, 1, 1);
            dateTime = dateTime.AddDays((double)version.Build);
            return dateTime.AddSeconds((double)(version.Revision * 2));
        }
    }
}
