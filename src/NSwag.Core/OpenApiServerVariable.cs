//-----------------------------------------------------------------------
// <copyright file="OpenApiServerVariable.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace NSwag
{
    /// <summary>Describes an OpenAPI server variable.</summary>
    public class OpenApiServerVariable
    {
        /// <summary>Gets or sets the enum of the server.</summary>
        [JsonPropertyName("enum")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<string> Enum { get; } = [];

        /// <summary>Gets or sets the variables of the server.</summary>
        [JsonPropertyName("default")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Default { get; set; }

        /// <summary>Gets or sets the description of the server.</summary>
        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Description { get; set; }
    }
}