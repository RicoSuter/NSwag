﻿//-----------------------------------------------------------------------
// <copyright file="OAuth2ClientSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;

#if AspNetOwin
namespace NSwag.AspNet.Owin
#else
namespace NSwag.AspNetCore
#endif
{
    /// <summary>The OAuth client settings.</summary>
    public class OAuth2ClientSettings
    {
        /// <summary>Gets or sets the client identifier.</summary>
        public string ClientId { get; set; } = "client_id";

        /// <summary>Gets or sets the client secret.</summary>
        public string ClientSecret { get; set; } = "client_secret";

        /// <summary>Gets or sets the realm.</summary>
        public string Realm { get; set; } = "realm";

        /// <summary>Gets or sets the name of the application.</summary>
        public string AppName { get; set; } = "app_name";

        /// <summary>Gets or sets the scope separator.</summary>
        public string ScopeSeparator { get; set; } = " ";

        /// <summary>Gets or sets initially selected scopes.</summary>
        public ICollection<string> Scopes { get; } = new List<string>();

        /// <summary>Gets or sets the additional query string parameters.</summary>
        public IDictionary<string, string> AdditionalQueryStringParameters { get; } = new Dictionary<string, string>();

        /// <summary>Proof Key for Code Exchange. Only applies to `accessCode` flow. Supported in SwaggerUI 3.</summary>
        public bool UsePkceWithAuthorizationCodeGrant { get; set; }
    }
}