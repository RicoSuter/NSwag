//-----------------------------------------------------------------------
// <copyright file="OpenApiExample.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Text.Json.Serialization;
using NJsonSchema.References;

namespace NSwag
{
    /// <summary>The Swagger example (OpenAPI only).</summary>
    public class OpenApiExample : JsonReferenceBase<OpenApiExample>, IJsonReference
    {
        /// <summary>Gets or sets the example's description.</summary>
        [JsonPropertyName("summary")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Summary { get; set; }

        /// <summary>Gets or sets the example's description.</summary>
        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Description { get; set; }

        /// <summary>Gets or sets the example's value.</summary>
        [JsonPropertyName("value")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public object Value { get; set; }

        /// <summary>Gets or sets the example's external value.</summary>
        [JsonPropertyName("externalValue")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string ExternalValue { get; set; }

        /// <summary>Gets the actual example, either this or the referenced example.</summary>
        [JsonIgnore]
        public OpenApiExample ActualExample => Reference ?? this;

        #region Implementation of IJsonReference

        [JsonIgnore]
        IJsonReference IJsonReference.ActualObject => ActualExample;

        [JsonIgnore]
        object IJsonReference.PossibleRoot => null;

        #endregion
    }
}