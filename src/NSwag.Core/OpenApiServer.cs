//-----------------------------------------------------------------------
// <copyright file="OpenApiServer.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;

namespace NSwag
{
    public class OpenApiServer
    {
        /// <summary>Gets or sets the URL of the server.</summary>
        [JsonProperty(PropertyName = "url", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Url { get; set; }

        /// <summary>Gets or sets the description of the server.</summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Description { get; set; }

        /// <summary>Gets or sets the variables of the server.</summary>
        [JsonProperty(PropertyName = "variables", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Variables { get; set; }
    }
}