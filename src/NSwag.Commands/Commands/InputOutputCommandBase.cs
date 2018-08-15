//-----------------------------------------------------------------------
// <copyright file="InputOutputCommandBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Infrastructure;

#pragma warning disable 1591

namespace NSwag.Commands
{
    public abstract class InputOutputCommandBase : OutputCommandBase
    {
        [JsonIgnore]
        [Argument(Name = "Input", IsRequired = true, AcceptsCommandInput = true, Description = "A file path or URL to the data or the JSON data itself.")]
        public object Input { get; set; }

        [Argument(Name = "ServiceHost", IsRequired = false, Description = "Overrides the service host of the web document (optional, use '.' to remove the hostname).")]
        public string ServiceHost { get; set; }

        [Argument(Name = "ServiceSchemes", IsRequired = false, Description = "Overrides the allowed schemes of the web service (optional, comma separated, 'http', 'https', 'ws', 'wss').")]
        public string[] ServiceSchemes { get; set; }

        /// <exception cref="ArgumentException">The argument 'Input' was empty.</exception>
        protected async Task<SwaggerDocument> GetInputSwaggerDocument()
        {
            var document = Input as SwaggerDocument;
            if (document == null)
            {
                var input = Input.ToString();

                if (string.IsNullOrEmpty(input))
                    throw new ArgumentException("The argument 'Input' was empty.");

                document = await ReadSwaggerDocumentAsync(input);
            }

            if (ServiceHost == ".")
            {
                document.Host = string.Empty;
                document.Schemes.Clear();
            }
            else
            {
                if (!string.IsNullOrEmpty(ServiceHost))
                    document.Host = ServiceHost;

                if (ServiceSchemes != null && ServiceSchemes.Any())
                    document.Schemes = ServiceSchemes.Select(s => (SwaggerSchema)Enum.Parse(typeof(SwaggerSchema), s, true)).ToList();
            }

            return document;
        }

        /// <exception cref="ArgumentException">The argument 'Input' was empty.</exception>
        protected async Task<JsonSchema4> GetJsonSchemaAsync()
        {
            var input = Input.ToString();
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("The argument 'Input' was empty.");

            if (IsJson(input))
                return await JsonSchema4.FromJsonAsync(input).ConfigureAwait(false);

            if (await DynamicApis.FileExistsAsync(input).ConfigureAwait(false))
                return await JsonSchema4.FromFileAsync(input).ConfigureAwait(false);

            return await JsonSchema4.FromUrlAsync(input).ConfigureAwait(false);
        }
    }
}