using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using NConsole;
using Newtonsoft.Json;

namespace NSwag.Commands.Base
{
    public abstract class InputOutputCommandBase : OutputCommandBase
    {
        [Description("A file path or URL to the data or the JSON data itself.")]
        [Argument(Name = "Input")]
        public string Input { get; set; }

        [JsonIgnore]
        protected SwaggerService InputSwaggerService
        {
            get
            {
                if (IsJson)
                    return SwaggerService.FromJson(Input);

                if (File.Exists(Input))
                    return SwaggerService.FromJson(File.ReadAllText(Input, Encoding.UTF8));

                return SwaggerService.FromUrl(Input);
            }
        }

        [JsonIgnore]
        protected string InputJson
        {
            get
            {
                if (IsJson)
                    return Input;

                if (File.Exists(Input))
                    return File.ReadAllText(Input, Encoding.UTF8);

                using (WebClient client = new WebClient())
                    return client.DownloadString(Input);
            }
        }

        private bool IsJson
        {
            get { return Input.Contains("{"); }
        }
    }
}