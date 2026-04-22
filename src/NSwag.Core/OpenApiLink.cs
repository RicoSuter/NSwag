//-----------------------------------------------------------------------
// <copyright file="OpenApiLink.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Text.Json.Serialization;
using NJsonSchema.References;

namespace NSwag
{
    /// <summary>The OpenApi link (OpenAPI only).</summary>
    public class OpenApiLink : JsonReferenceBase<OpenApiLink>, IJsonReference
    {
        /// <summary>Gets or sets the example's description.</summary>
        [JsonPropertyName("operationRef")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string OperationRef { get; set; }

        /// <summary>Gets or sets the example's description.</summary>
        [JsonPropertyName("operationId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string OperationId { get; set; }

        /// <summary>Gets or sets the example's value.</summary>
        [JsonPropertyName("parameters")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IDictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        /// <summary>Gets or sets the example's external value.</summary>
        [JsonPropertyName("requestBody")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public object RequestBody { get; set; }

        /// <summary>Gets or sets the example's external value.</summary>
        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Description { get; set; }

        /// <summary>Gets or sets the server.</summary>
        [JsonPropertyName("server")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public OpenApiServer Server { get; set; }

        /// <summary>Gets the actual link, either this or the referenced example.</summary>
        [JsonIgnore]
        public OpenApiLink ActualLink => Reference ?? this;

        #region Implementation of IJsonReference

        [JsonIgnore]
        IJsonReference IJsonReference.ActualObject => ActualLink;

        [JsonIgnore]
        object IJsonReference.PossibleRoot => null;

        #endregion
    }
}