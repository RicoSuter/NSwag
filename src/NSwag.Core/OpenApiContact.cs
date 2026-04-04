//-----------------------------------------------------------------------
// <copyright file="SwaggerContact.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Text.Json.Serialization;
using NJsonSchema;

namespace NSwag
{
    /// <summary>The web service contact description.</summary>
    public class OpenApiContact : JsonExtensionObject
    {
        /// <summary>Gets or sets the name.</summary>
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Name { get; set; }

        /// <summary>Gets or sets the contact URL.</summary>
        [JsonPropertyName("url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Url { get; set; }

        /// <summary>Gets or sets the contact email.</summary>
        [JsonPropertyName("email")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Email { get; set; }
    }
}