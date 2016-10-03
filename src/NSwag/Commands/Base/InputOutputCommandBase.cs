using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using NConsole;
using Newtonsoft.Json;

namespace NSwag.Commands.Base
{
    public abstract class InputOutputCommandBase : OutputCommandBase
    {
        [JsonIgnore]
        [Description("A file path or URL to the data or the JSON data itself.")]
        [Argument(Name = "Input", IsRequired = true, AcceptsCommandInput = true)]
        public object Input { get; set; }

        [Description("Overrides the service host of the web service (optional).")]
        [Argument(Name = "ServiceHost", IsRequired = false)]
        public string ServiceHost { get; set; }

        [Description("Overrides the allowed schemes of the web service (optional, comma separated, 'http', 'https', 'ws', 'wss').")]
        [Argument(Name = "ServiceSchemes", IsRequired = false)]
        public string[] ServiceSchemes { get; set; }

        /// <exception cref="ArgumentException" accessor="get">The argument 'Input' was empty.</exception>
        [JsonIgnore]
        protected SwaggerService InputSwaggerService
        {
            get
            {
                var swaggerService = Input as SwaggerService;
                if (swaggerService == null)
                {
                    var inputString = Input.ToString();
                    if (string.IsNullOrEmpty(inputString))
                        throw new ArgumentException("The argument 'Input' was empty.");

                    if (IsJson(inputString))
                        swaggerService = SwaggerService.FromJson(inputString);
                    else 
                        swaggerService = SwaggerService.FromUrl(inputString);
                }

                if (!string.IsNullOrEmpty(ServiceHost))
                    swaggerService.Host = ServiceHost;
                if (ServiceSchemes != null && ServiceSchemes.Any())
                    swaggerService.Schemes = ServiceSchemes.Select(s => (SwaggerSchema)Enum.Parse(typeof(SwaggerSchema), s, true)).ToList();

                return swaggerService; 
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