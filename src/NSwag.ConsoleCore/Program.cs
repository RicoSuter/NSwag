using System;
using System.Threading.Tasks;
using NSwag.Commands;

namespace NSwag
{
    public class Program
    {
        private static Task<int> Main(string[] args)
        {
            Console.Write("NSwag command line tool for .NET Core " + RuntimeUtilities.CurrentRuntime + ", ");
            var processor = new NSwagCommandProcessor(new CoreConsoleHost());
            return processor.ProcessAsync(args);
        }
    }
}
