//-----------------------------------------------------------------------
// <copyright file="SwaggerUiSettingsBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
#if AspNetOwin
using Microsoft.Owin;

namespace NSwag.AspNet.Owin
#else
using Microsoft.AspNetCore.Http;

namespace NSwag.AspNetCore
#endif
{
    internal static class HttpRequestExtension
    {
#if AspNetOwin
        private static string GetHttpScheme(this IOwinRequest request)
#else
        private static string GetHttpScheme(this HttpRequest request)
#endif
        {
            return request.Headers.TryGetFirstHeader("X-Forwarded-Proto") ?? request.Scheme;
        }

#if AspNetOwin
        public static string GetServerUrl(this IOwinRequest request)
#else
        public static string GetServerUrl(this HttpRequest request)
#endif
        {
            var baseUrl = request.Headers.ContainsKey("X-Forwarded-Host") ?
                new Uri($"{request.GetHttpScheme()}://{request.Headers.TryGetFirstHeader("X-Forwarded-Host")}").ToString().TrimEnd('/') :
                new Uri($"{request.GetHttpScheme()}://{request.Host}").ToString().TrimEnd('/');

            return $"{baseUrl}{request.GetBasePath()}".TrimEnd('/');
        }

#if AspNetOwin
        public static string GetBasePath(this IOwinRequest request)
#else
        public static string GetBasePath(this HttpRequest request)
#endif
        {
            if (request.Headers.ContainsKey("X-Forwarded-Prefix"))
            {
                return "/" + request.Headers.TryGetFirstHeader("X-Forwarded-Prefix").Trim('/');
            }

            var basePath = request.Headers.ContainsKey("X-Forwarded-Host") ?
                new Uri($"http://{request.Headers.TryGetFirstHeader("X-Forwarded-Host")}").AbsolutePath :
                "";

            if (request.PathBase.HasValue)
            {
                basePath = basePath.TrimEnd('/') + "/" + request.PathBase.Value;
            }

            return ("/" + basePath.Trim('/')).TrimEnd('/');
        }

        private static string TryGetFirstHeader(this IHeaderDictionary headers, string name)
        {
#if AspNetOwin
            return headers[name]?.Split(',').Select(s => s.Trim()).First();
#else
            return headers[name].FirstOrDefault()?.Split(',').Select(s => s.Trim()).First();
#endif
        }
    }
}
