using NConsole;
using NSwag.CodeGeneration;
using NSwag.Commands;

namespace NSwag
{
    public class Program
    {
        static int Main(string[] args)
        {
            var processor = new NSwagCommandProcessor(typeof(NSwagDocument).Assembly, new ConsoleHost());
            return processor.Process(args);
        }
    }
}
