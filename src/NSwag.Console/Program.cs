using System;
using NConsole;
using NSwag.Commands;

namespace NSwag
{
    public class Program
    {
        static int Main(string[] args)
        {
            Console.Write("NSwag command line tool for .NET 4.6.1+, ");
            var processor = new NSwagCommandProcessor(typeof(NSwagDocument).Assembly, new ConsoleHost());
            return processor.Process(args);
        }
    }
}
