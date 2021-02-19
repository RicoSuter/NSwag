//-----------------------------------------------------------------------
// <copyright file="OpenApiExample.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;
using NJsonSchema.References;

namespace NSwag
{
    /// <summary>The Swagger example (OpenAPI only).</summary>
    public class OpenApiExample : JsonReferenceBase<OpenApiExample>, IJsonReference
    {
        /// <summary>Gets or sets the example's description.</summary>
        [JsonProperty(PropertyName = "summary", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Summary { get; set; }

        /// <summary>Gets or sets the example's description.</summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Description { get; set; }

        /// <summary>Gets or sets the example's value.</summary>
        [JsonProperty(PropertyName = "value", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public object Value { get; set; }

        /// <summary>Gets or sets the example's external value.</summary>
        [JsonProperty(PropertyName = "externalValue", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
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