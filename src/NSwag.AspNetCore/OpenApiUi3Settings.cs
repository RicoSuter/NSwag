//-----------------------------------------------------------------------
// <copyright file="OpenApiUi3Settings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace NSwag.AspNetCore
{
    /// <summary>
    ///
    /// </summary>
    public class OpenApiUi3Settings
    {
        /// <summary>
        /// Gets or sets the Open API UI route template for all registered documents. Must start with '/'.
        /// </summary>
        /// <value>Default is <c>"/swagger"</c>.</value>
        public string UiRouteTemplate { get; set; } = "/swagger";

        /// <summary>
        /// Gets a collection of additional <see cref="SwaggerUi3Route"/>s to display in the UI. Should not include
        /// information about registered Open API documents in this application.
        /// </summary>
        public ICollection<SwaggerUi3Route> AdditionalSwaggerRoutes { get; } = new List<SwaggerUi3Route>();

        /// <summary>Gets or sets a value indicating whether the Swagger specification should be validated.</summary>
        /// <value>Default is <see langword="false"/>.</value>
        public bool ValidateSpecification { get; set; }

        /// <summary>Gets or sets the Swagger UI OAuth2 client settings.</summary>
        /// <value>Default is <see langword="null"/>.</value>
        public OAuth2ClientSettings OAuth2Client { get; set; }

        /// <summary>
        /// Controls how the API listing is displayed. It can be set to 'none' (default), 'list' (shows operations for
        /// each resource), or 'full' (fully expanded: shows operations and their details).
        /// </summary>
        /// <value>Default is <c>"none"</c>.</value>
        public string DocExpansion { get; set; } = "none";

        /// <summary>
        /// Specifies the API sorter in Swagger UI 3. May be set to 'none' (default), 'alpha', or a JavaScript
        /// function.
        /// </summary>
        /// <value>Default is <c>"none"</c>.</value>
        public string ApisSorter { get; set; } = "none";

        /// <summary>
        /// Specifies the operations sorter in Swagger UI 3. May be set to 'none' (default), 'alpha', 'method', or a
        /// JavaScript function.
        /// </summary>
        /// <value>Default is <c>"none"</c>.</value>
        public string OperationsSorter { get; set; } = "none";

        /// <summary>
        /// The default expansion depth for models (set to -1 completely hide the models) in Swagger UI 3.
        /// </summary>
        /// <value>Default is <c>1</c>.</value>
        public int DefaultModelsExpandDepth { get; set; } = 1;

        /// <summary>The default expansion depth for the model on the model-example section in Swagger UI 3.</summary>
        /// <value>Default is <c>1</c>.</value>
        public int DefaultModelExpandDepth { get; set; } = 1;

        /// <summary>Specifies the tag sorter in Swagger UI 3</summary>
        /// <value>Default is <c>"none"</c>.</value>
        public string TagSorter { get; set; } = "none";

        /// <summary>Specifies whether the "Try it out" option is enabled in Swagger UI 3.</summary>
        /// <value>Default is <see langword="true"/>.</value>
        public bool EnableTryItOut { get; set; } = true;

        /// <summary>
        /// Gets or sets the server URL. Used when returning to the site after an OAuth authorization.
        /// </summary>
        /// <value>
        /// Default is the empty string, indicating the return URI should use the browser's location information.
        /// </value>
        public string ServerUrl { get; set; } = "";
    }
}
