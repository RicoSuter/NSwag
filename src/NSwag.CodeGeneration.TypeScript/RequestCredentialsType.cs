//-----------------------------------------------------------------------
// <copyright file="RequestCredentialsType.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.TypeScript
{
    /// <summary>Settings for the <see cref="TypeScriptClientGenerator"/>.</summary>
    public enum RequestCredentialsType
    {
        /// https://developer.mozilla.org/en-US/docs/Web/API/RequestInit#credentials

        /// <summary>
        /// Setting will not be included.
        /// </summary>
        NotSet,

        /// <summary>
        /// Never send credentials in the request or include credentials in the response.
        /// </summary>
        Omit,

        /// <summary>
        /// Only send and include credentials for same-origin requests.   
        /// </summary>
        SameOrigin,

        /// <summary>
        /// Always include credentials, even for cross-origin requests.
        /// </summary>
        Include
    }
}
