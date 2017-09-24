using System;
using System.Reflection;
using NSwag.Commands;

namespace NSwag
{
    public class Program
    {
        static int Main(string[] args)
        {
            Console.Write("NSwag command line tool for .NET Core " + RuntimeUtilities.CurrentRuntime + ", ");
            var processor = new NSwagCommandProcessor(new CoreConsoleHost());
            return processor.Process(args);
        }
    }
}
