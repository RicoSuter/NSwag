using System;
using System.Threading.Tasks;
using NConsole;
using NSwag.Commands;

namespace NSwag
{
    public class Program
    {
        private static Task<int> Main(string[] args)
        {
            Console.Write("NSwag command line tool for .NET 4.6.2+ " + RuntimeUtilities.CurrentRuntime + ", ");
            var processor = new NSwagCommandProcessor(new ConsoleHost());
            return processor.ProcessAsync(args);
        }
    }
}
