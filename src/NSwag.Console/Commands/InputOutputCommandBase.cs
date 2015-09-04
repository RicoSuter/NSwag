using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NConsole;

namespace NSwag.Console.Commands
{
    public abstract class InputOutputCommandBase : IConsoleCommand
    {
        [Description("An URL to the Swagger definition or the JSON itself.")]
        [Argument(Position = 1)]
        public string Input { get; set; }

        [Description("The output file path (optional).")]
        [Argument(Name = "Output", DefaultValue = "")]
        public string Output { get; set; }

        public abstract Task RunAsync(CommandLineProcessor processor, IConsoleHost host);

        protected SwaggerService InputSwaggerService
        {
            get { return Input.Contains("{") ? SwaggerService.FromJson(Input) : SwaggerService.FromUrl(Input); }
        }

        protected void WriteOutput(IConsoleHost host, string output)
        {
            if (string.IsNullOrEmpty(Output))
                host.WriteMessage(output);
            else
            {
                File.WriteAllText(Output, output, Encoding.UTF8);
                host.WriteMessage("Client code successfully written to file.");
            }
        }
    }
}