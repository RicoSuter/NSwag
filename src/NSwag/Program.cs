using System;
using System.Diagnostics;
using NConsole;
using NSwag.Commands;

namespace NSwag
{
    public class Program
    {
        static void Main(string[] args)
        {
            var host = new ConsoleHost();
            host.WriteMessage("NSwag command line: v" + typeof(SwaggerInfo).Assembly.GetName().Version + "\n");
            host.WriteMessage("Visit http://NSwag.org for more information.\n");
            host.WriteMessage("Execute the 'help' command to show a list of all the available commands.\n");

            try
            {
                var processor = new CommandLineProcessor(host);

                processor.RegisterCommand<WebApiToSwaggerCommand>("webapi2swagger");

                processor.RegisterCommand<JsonSchemaToCSharpCommand>("jsonschema2csharp");
                processor.RegisterCommand<JsonSchemaToTypeScriptCommand>("jsonschema2typescript");
                
                processor.RegisterCommand<SwaggerToCSharpClientCommand>("swagger2csharp");
                processor.RegisterCommand<SwaggerToTypeScriptCommand>("swagger2typescript");

                processor.Process(args);
            }
            catch (Exception exception)
            {
                var savedForegroundColor = Console.ForegroundColor; 
                Console.ForegroundColor = ConsoleColor.Red;
                host.WriteMessage(exception.ToString());
                Console.ForegroundColor = savedForegroundColor;
            }

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press <any> key to exit...");
                Console.ReadKey();
            }
        }
    }
}
