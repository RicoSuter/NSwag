using System.ComponentModel;
using System.IO;
using System.Text;
using NConsole;

namespace NSwag.Commands
{
    public abstract class InputOutputCommandBase : OutputCommandBase
    {
        [Description("An URL to the Swagger definition or the JSON itself.")]
        [Argument(Name = "Input")]
        public string Input { get; set; }

        protected SwaggerService InputSwaggerService
        {
            get
            {
                if (Input.Contains("{"))
                    return SwaggerService.FromJson(Input);
                if (File.Exists(Input))
                    return SwaggerService.FromJson(File.ReadAllText(Input, Encoding.UTF8));
                return SwaggerService.FromUrl(Input);
            }
        }
    }
}