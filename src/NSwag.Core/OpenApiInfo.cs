//-----------------------------------------------------------------------
// <copyright file="SwaggerInfo.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Text.Json.Serialization;
using NJsonSchema;

namespace NSwag
{
    /// <summary>The web service description.</summary>
    public class OpenApiInfo : JsonExtensionObject
    {
        /// <summary>Gets or sets the title.</summary>
        [JsonPropertyName("title")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonRequired]
        public string Title { get; set; } = "Swagger specification";

        /// <summary>Gets or sets the description.</summary>
        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Description { get; set; }

        /// <summary>Gets or sets the terms of service.</summary>
        [JsonPropertyName("termsOfService")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string TermsOfService { get; set; }

        /// <summary>Gets or sets the contact information.</summary>
        [JsonPropertyName("contact")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public OpenApiContact Contact { get; set; }

        /// <summary>Gets or sets the license information.</summary>
        [JsonPropertyName("license")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public OpenApiLicense License { get; set; }

        /// <summary>Gets or sets the API version.</summary>
        [JsonPropertyName("version")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonRequired]
        public string Version { get; set; } = "1.0.0";
    }
}