//-----------------------------------------------------------------------
// <copyright file="SwaggerTag.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Text.Json.Serialization;
using NJsonSchema;

namespace NSwag
{
    /// <summary>Describes a Swagger tag.</summary>
    public class OpenApiTag : JsonExtensionObject
    {
        /// <summary>Gets or sets the name.</summary>
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Name { get; set; }

        /// <summary>Gets or sets the description.</summary>
        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Description { get; set; }

        /// <summary>Gets or sets the external documentation.</summary>
        [JsonPropertyName("externalDocs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public OpenApiExternalDocumentation ExternalDocumentation { get; set; }
    }
}