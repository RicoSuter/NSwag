//-----------------------------------------------------------------------
// <copyright file="InputOutputCommandBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
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
        protected async Task<OpenApiDocument> GetInputSwaggerDocument()
        {
            var document = Input as OpenApiDocument;
            if (document == null)
            {
                var input = Input.ToString();

                if (string.IsNullOrEmpty(input))
                {
                    throw new ArgumentException("The argument 'Input' was empty.");
                }

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
                {
                    document.Host = ServiceHost;
                }

                if (ServiceSchemes != null && ServiceSchemes.Any())
                {
                    document.Schemes = ServiceSchemes.Select(s => (OpenApiSchema)Enum.Parse(typeof(OpenApiSchema), s, true)).ToList();
                }
            }

            return document;
        }

        /// <exception cref="ArgumentException">The argument 'Input' was empty.</exception>
        protected async Task<JsonSchema> GetJsonSchemaAsync()
        {
            var input = Input.ToString();
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("The argument 'Input' was empty.");
            }

            if (IsJson(input))
            {
                return await JsonSchema.FromJsonAsync(input).ConfigureAwait(false);
            }

            if (DynamicApis.FileExists(input))
            {
                return await JsonSchema.FromFileAsync(input).ConfigureAwait(false);
            }

            return await JsonSchema.FromUrlAsync(input).ConfigureAwait(false);
        }
    }
}