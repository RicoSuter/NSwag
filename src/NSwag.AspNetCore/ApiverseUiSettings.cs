//-----------------------------------------------------------------------
// <copyright file="ApiverseUiSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Microsoft.AspNetCore.Http;

namespace NSwag.AspNetCore
{
    /// <summary>
    /// The Apiverse.io UI settings.
    /// </summary>
    public class ApiverseUiSettings : SwaggerUiSettingsBase
    {
        /// <summary>Initializes a new instance of the <see cref="ApiverseUiSettings"/> class.</summary>
        public ApiverseUiSettings()
        {
            DocumentPath = "swagger/v1/swagger.json";
        }

        /// <summary>
        /// Gets or sets the global document ID to compare with.
        /// </summary>
        public string CompareWith { get; set; }

        /// <summary>
        /// Gets or sets the Apiverse.io instance URL.
        /// </summary>
        public string ApiverseUrl { get; set; } = "https://apiverse.io";

        internal override string TransformHtml(string html, HttpRequest request)
        {
            return html;
        }
    }
}
