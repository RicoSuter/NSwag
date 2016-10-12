//-----------------------------------------------------------------------
// <copyright file="SwaggerInfo.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;

namespace NSwag
{
    /// <summary>The web service description.</summary>
    public class SwaggerInfo
    {
        /// <summary>Gets or sets the title.</summary>
        [JsonProperty(PropertyName = "title", Required = Required.Always,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Title { get; set; } = "Swagger specification";

        /// <summary>Gets or sets the description.</summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Description { get; set; }

        /// <summary>Gets or sets the terms of service.</summary>
        [JsonProperty(PropertyName = "termsOfService", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string TermsOfService { get; set; }

        /// <summary>Gets or sets the contact information.</summary>
        [JsonProperty(PropertyName = "contact", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public SwaggerContact Contact { get; set; }

        /// <summary>Gets or sets the license information.</summary>
        [JsonProperty(PropertyName = "license", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public SwaggerLicense License { get; set; }

        /// <summary>Gets or sets the API version.</summary>
        [JsonProperty(PropertyName = "version", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Version { get; set; } = "1.0.0";

        /// <summary>Get or set the schema less extensions (this can be used as vendor extensions as well) in Path schema</summary>
        [JsonExtensionData]
        public IDictionary<string, object> ExtensionData { get; set; }

    }
}