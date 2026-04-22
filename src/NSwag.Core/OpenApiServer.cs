//-----------------------------------------------------------------------
// <copyright file="OpenApiServer.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace NSwag
{
    /// <summary>Describes an OpenAPI server.</summary>
    public class OpenApiServer
    {
        /// <summary>Gets or sets the URL of the server.</summary>
        [JsonPropertyName("url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Url { get; set; }

        /// <summary>Gets or sets the description of the server.</summary>
        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Description { get; set; }

        /// <summary>Gets or sets the variables of the server.</summary>
        [JsonPropertyName("variables")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IDictionary<string, OpenApiServerVariable> Variables { get; } = new Dictionary<string, OpenApiServerVariable>();

        /// <summary>Gets a value indicating whether the server description is valid.</summary>
        [JsonIgnore]
        public bool IsValid => !string.IsNullOrEmpty(Url) && !Url.Contains("///");
    }
}