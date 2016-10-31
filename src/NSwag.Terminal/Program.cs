using NSwag.CodeGeneration;
using NSwag.Commands;

namespace NSwag
{
    public class Program
    {
        static int Main(string[] args)
        {
            var processor = new NSwagCommandProcessor(typeof(NSwagDocument).Assembly);
            return processor.Process(args);
        }
    }
}
