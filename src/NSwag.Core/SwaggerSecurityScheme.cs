//-----------------------------------------------------------------------
// <copyright file="SwaggerSecurityScheme.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NSwag
{
    /// <summary>The definition of a security scheme that can be used by the operations.</summary>
    public class SwaggerSecurityScheme
    {
        /// <summary>Gets or sets the type of the security scheme.</summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [JsonConverter(typeof(StringEnumConverter))]
        public SwaggerSecuritySchemeType Type { get; set; }

        /// <summary>Gets or sets the short description for security scheme.</summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Description { get; set; }

        /// <summary>Gets or sets the name of the header or query parameter to be used to transmit the API key.</summary>
        [JsonProperty(PropertyName = "name", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Name { get; set; }
        
        /// <summary>Gets or sets the type of the API key.</summary>
        [JsonProperty(PropertyName = "in", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [JsonConverter(typeof(StringEnumConverter))]
        public SwaggerSecurityApiKeyLocation In { get; set; }

        /// <summary>Gets or sets the used by the OAuth2 security scheme.</summary>
        [JsonProperty(PropertyName = "flow", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public SwaggerOAuth2Flow Flow { get; set; }

        /// <summary>Gets or sets the authorization URL to be used for this flow.</summary>
        [JsonProperty(PropertyName = "authorizationUrl", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string AuthorizationUrl { get; set; }

        /// <summary>Gets or sets the token URL to be used for this flow. .</summary>
        [JsonProperty(PropertyName = "tokenUrl", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string TokenUrl { get; set; }

        /// <summary>Gets the available scopes for the OAuth2 security scheme.</summary>
        [JsonProperty(PropertyName = "scopes", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, string> Scopes { get; set; }

        /// <summary>Get or set the schema less extensions (this can be used as vendor extensions as well) in security schema</summary>
        [JsonExtensionData]
        public IDictionary<string, object> ExtensionData { get; set; }

    }
}