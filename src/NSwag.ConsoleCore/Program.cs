using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using NSwag.CodeGeneration;
using NSwag.Commands;

namespace NSwag
{
    public class Program
    {
        static int Main(string[] args)
        {
            Directory.SetCurrentDirectory("C:\\Data\\Timely.Web\\src\\Timely.Aurelia");

            var processor = new NSwagCommandProcessor(typeof(NSwagDocument).GetTypeInfo().Assembly, new CoreConsoleHost());
            return processor.Process(new[] { "run" });
        }
    }
}
