//-----------------------------------------------------------------------
// <copyright file="OpenApiLink.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;
using NJsonSchema.References;

namespace NSwag
{
    /// <summary>The OpenApi link (OpenAPI only).</summary>
    public class OpenApiLink : JsonReferenceBase<OpenApiLink>, IJsonReference
    {
        /// <summary>Gets or sets the example's description.</summary>
        [JsonProperty(PropertyName = "operationRef", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string OperationRef { get; set; }

        /// <summary>Gets or sets the example's description.</summary>
        [JsonProperty(PropertyName = "operationId", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string OperationId { get; set; }

        /// <summary>Gets or sets the example's value.</summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        /// <summary>Gets or sets the example's external value.</summary>
        [JsonProperty(PropertyName = "requestBody", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public object RequestBody { get; set; }

        /// <summary>Gets or sets the example's external value.</summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Description { get; set; }

        /// <summary>Gets or sets the server.</summary>
        [JsonProperty(PropertyName = "server", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
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