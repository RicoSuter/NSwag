//-----------------------------------------------------------------------
// <copyright file="ApimundoUiSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NSwag.AspNetCore
{
    /// <summary>
    /// The Apimundo UI settings.
    /// </summary>
    public class ApimundoUiSettings : SwaggerUiSettingsBase
    {
        /// <summary>Initializes a new instance of the <see cref="ApimundoUiSettings"/> class.</summary>
        public ApimundoUiSettings()
        {
            DocumentPath = "swagger/v1/swagger.json";
        }

        /// <summary>
        /// Gets or sets the global document ID to compare with.
        /// </summary>
        public string CompareWith { get; set; }

        /// <summary>
        /// Gets or sets the Apimundo instance URL.
        /// </summary>
        public string ApimundoUrl { get; set; } = "https://apimundo.com";

        internal override Task<string> TransformHtmlAsync(string html, HttpRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(html);
        }
    }
}
