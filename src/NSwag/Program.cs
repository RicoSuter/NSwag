using System;
using System.Diagnostics;
using NConsole;
using NSwag.Console.Commands;

namespace NSwag
{
    public class Program
    {
        static void Main(string[] args)
        {
            var host = new ConsoleHost();
            host.WriteMessage("NSwag command line: v" + typeof(SwaggerInfo).Assembly.GetName().Version + "\n");
            host.WriteMessage("Visit http://NSwag.org for more information.");
            host.WriteMessage("\n");

            try
            {
                var processor = new CommandLineProcessor(host);

                processor.RegisterCommand<CSharpCommand>("csharp");
                processor.RegisterCommand<TypeScriptCommand>("typescript");

                processor.Process(args);
            }
            catch (Exception exception)
            {
                var savedForegroundColor = System.Console.ForegroundColor; 
                System.Console.ForegroundColor = ConsoleColor.Red;
                host.WriteMessage(exception.ToString());
                System.Console.ForegroundColor = savedForegroundColor;
            }

            if (Debugger.IsAttached)
                System.Console.ReadKey();
        }
    }
}
