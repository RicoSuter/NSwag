using System;
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
        [Argument(Name = "Input", IsRequired = true, AcceptsCommandInput = true)]
        public object Input { get; set; }

        /// <exception cref="ArgumentException" accessor="get">The argument 'Input' was empty.</exception>
        [JsonIgnore]
        protected SwaggerService InputSwaggerService
        {
            get
            {
                var swaggerService = Input as SwaggerService;
                if (swaggerService != null)
                    return swaggerService;

                var inputString = Input.ToString();
                if (string.IsNullOrEmpty(inputString))
                    throw new ArgumentException("The argument 'Input' was empty.");

                if (IsJson(inputString))
                    return SwaggerService.FromJson(inputString);

                if (File.Exists(inputString))
                    return SwaggerService.FromFile(inputString);

                return SwaggerService.FromUrl(inputString);
            }
        }

        /// <exception cref="ArgumentException" accessor="get">The argument 'Input' was empty.</exception>
        [JsonIgnore]
        protected string InputJson
        {
            get
            {
                var inputString = Input.ToString();
                if (string.IsNullOrEmpty(inputString))
                    throw new ArgumentException("The argument 'Input' was empty.");

                if (IsJson(inputString))
                    return inputString;

                if (File.Exists(inputString))
                    return File.ReadAllText(inputString, Encoding.UTF8);

                using (WebClient client = new WebClient())
                    return client.DownloadString(inputString);
            }
        }

        private bool IsJson(string data)
        {
            return !string.IsNullOrEmpty(data) && data.Contains("{");
        }
    }
}