//-----------------------------------------------------------------------
// <copyright file="SwaggerExternalDocumentation.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;
using NJsonSchema;

namespace NSwag
{
    /// <summary>The external documentation description.</summary>
    public class OpenApiExternalDocumentation : JsonExtensionObject
    {
        /// <summary>Gets or sets the description.</summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Description { get; set; }

        /// <summary>Gets or sets the documentation URL.</summary>
        [JsonProperty(PropertyName = "url", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Url { get; set; }
    }
}