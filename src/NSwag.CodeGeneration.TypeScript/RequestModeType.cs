//-----------------------------------------------------------------------
// <copyright file="RequestModeType.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.TypeScript
{
    /// <summary>Settings for the <see cref="TypeScriptClientGenerator"/>.</summary>
    public enum RequestModeType
    {
        /// https://developer.mozilla.org/en-US/docs/Web/API/RequestInit#mode

        /// <summary>
        /// Setting will not be included.
        /// </summary>
        NotSet,

        /// <summary>
        /// Disallows cross-origin requests. If a same-origin request is sent to a different origin, the result is a network error.
        /// </summary>
        SameOrigin,

        /// <summary>
        /// If the request is cross-origin then it will use the Cross-Origin Resource Sharing (CORS) mechanism. Only CORS-safelisted response headers are exposed in the response.
        /// </summary>
        Cors,

        /// <summary>
        /// Disables CORS for cross-origin requests. This option comes with the following restrictions: 
        /// 
        /// * The method may only be one of HEAD, GET or POST.
        /// * The headers may only be CORS-safelisted request headers, with the additional restriction that the Range header is also not allowed. This also applies to any headers added by service workers.
        /// * The response is opaque, meaning that its headers and body are not available to JavaScript, and its status code is always 0.
        /// </summary>
        NoCors,

        /// <summary>
        /// Used only by HTML navigation. A navigate request is created only while navigating between documents.
        /// </summary>
        Navigate,
    }
}
