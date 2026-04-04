//-----------------------------------------------------------------------
// <copyright file="SwaggerExternalDocumentation.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Text.Json.Serialization;
using NJsonSchema;

namespace NSwag
{
    /// <summary>The external documentation description.</summary>
    public class OpenApiExternalDocumentation : JsonExtensionObject
    {
        /// <summary>Gets or sets the description.</summary>
        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Description { get; set; }

        /// <summary>Gets or sets the documentation URL.</summary>
        [JsonPropertyName("url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Url { get; set; }
    }
}