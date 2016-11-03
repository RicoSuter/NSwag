using System.Reflection;
using NSwag.CodeGeneration;
using NSwag.Commands;

namespace NSwag
{
    public class Program
    {
        static int Main(string[] args)
        {
            var processor = new NSwagCommandProcessor(typeof(NSwagDocument).GetTypeInfo().Assembly, new CoreConsoleHost());
            return processor.Process(args);
        }
    }
}
