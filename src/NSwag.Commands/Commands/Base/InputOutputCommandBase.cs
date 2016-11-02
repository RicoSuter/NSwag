//-----------------------------------------------------------------------
// <copyright file="InputOutputCommandBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using NConsole;
using Newtonsoft.Json;
using NJsonSchema.Infrastructure;

#pragma warning disable 1591

namespace NSwag.Commands.Base
{
    public abstract class InputOutputCommandBase : OutputCommandBase
    {
        [JsonIgnore]
        [Argument(Name = "Input", IsRequired = true, AcceptsCommandInput = true, Description = "A file path or URL to the data or the JSON data itself.")]
        public object Input { get; set; }

        [Argument(Name = "ServiceHost", IsRequired = false, Description = "Overrides the service host of the web service (optional, use '.' to remove the hostname).")]
        public string ServiceHost { get; set; }

        [Argument(Name = "ServiceSchemes", IsRequired = false, Description = "Overrides the allowed schemes of the web service (optional, comma separated, 'http', 'https', 'ws', 'wss').")]
        public string[] ServiceSchemes { get; set; }

        /// <exception cref="ArgumentException" accessor="get">The argument 'Input' was empty.</exception>
        [JsonIgnore]
        protected SwaggerService InputSwaggerService
        {
            get
            {
                var service = Input as SwaggerService;
                if (service == null)
                {
                    var inputString = Input.ToString();
                    if (string.IsNullOrEmpty(inputString))
                        throw new ArgumentException("The argument 'Input' was empty.");

                    if (IsJson(inputString))
                        service = SwaggerService.FromJson(inputString);
                    else 
                        service = SwaggerService.FromUrl(inputString);
                }

                if (ServiceHost == ".")
                    service.Host = string.Empty;
                else if (!string.IsNullOrEmpty(ServiceHost))
                    service.Host = ServiceHost;

                if (ServiceSchemes != null && ServiceSchemes.Any())
                    service.Schemes = ServiceSchemes.Select(s => (SwaggerSchema)Enum.Parse(typeof(SwaggerSchema), s, true)).ToList();

                return service; 
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

                if (DynamicApis.FileExists(inputString))
                    return DynamicApis.FileReadAllText(inputString);

                return DynamicApis.HttpGet(inputString);
            }
        }

        private bool IsJson(string data)
        {
            return !string.IsNullOrEmpty(data) && data.Contains("{");
        }
    }
}