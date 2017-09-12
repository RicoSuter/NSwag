//-----------------------------------------------------------------------
// <copyright file="SwaggerUiOwinSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if AspNetOwin
namespace NSwag.AspNet.Owin
#else
namespace NSwag.AspNetCore
#endif
{
    /// <summary>The settings for UseSwaggerUi.</summary>
    public class SwaggerUiSettings : SwaggerUiSettingsBase
    {
        /// <summary>Gets or sets a value indicating whether the Swagger specification should be validated.</summary>
        public bool ValidateSpecification { get; set; } = true;

        /// <summary>Gets or sets the Swagger UI OAuth2 client settings.</summary>
        public OAuth2ClientSettings OAuth2Client { get; set; }

        /// <summary>Gets or sets the Swagger UI supported submit methods. An array of of the HTTP operations that will have the 'Try it out!' option.</summary>
        public string[] SupportedSubmitMethods { get; set; } = new[] { "get", "post", "put", "delete", "patch" };

        /// <summary>Controls how the API listing is displayed. It can be set to 'none' (default), 'list' (shows operations for each resource), or 'full' (fully expanded: shows operations and their details).</summary>
        public string DocExpansion { get; set; } = "none";

        /// <summary>Enables a graphical view for editing complex bodies. Defaults to false.</summary>
        public bool UseJsonEditor { get; set; } = false;

        /// <summary>Controls how models are shown when the API is first rendered. (The user can always switch the rendering for a given model by clicking the 'Model' and 'Model Schema' links.) It can be set to 'model' or 'schema', and the default is 'schema'.</summary>
        public string DefaultModelRendering { get; set; } = "schema";

        /// <summary>Whether or not to show the headers that were sent when making a request via the 'Try it out!' option. Defaults to false.</summary>
        public bool ShowRequestHeaders { get; set; } = false;
    }
}