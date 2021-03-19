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
        private const string ForwardedProtoHeader = "X-Forwarded-Proto";
        private const string ForwardedPrefixHeader = "X-Forwarded-Prefix";
        private const string ForwardedHostHeader = "X-Forwarded-Host";

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
            var host = TryGetFirstHeader(request.Headers, ForwardedHostHeader);
            var baseUrl = new Uri($"{request.GetHttpScheme()}://{host ?? request.Host.ToString()}").ToString().TrimEnd('/');

            return $"{baseUrl}{request.GetBasePath()}".TrimEnd('/');
        }

#if AspNetOwin
        public static string GetBasePath(this IOwinRequest request)
#else
        public static string GetBasePath(this HttpRequest request)
#endif
        {
            var prefix = TryGetFirstHeader(request.Headers, ForwardedPrefixHeader);

            if (prefix != null)
            {
                return "/" + prefix.Trim('/');
            }

            var host = TryGetFirstHeader(request.Headers, ForwardedHostHeader);
            string basePath;

            if (host != null)
            {
                var proto = TryGetFirstHeader(request.Headers, ForwardedProtoHeader) ?? "http";
                basePath = new Uri($"{proto}://{host}").AbsolutePath.TrimEnd('/');
            }
            else
            {
                basePath = "";
            }

            if (request.PathBase.HasValue)
            {
                basePath += "/" + request.PathBase.Value;
            }

            return ("/" + basePath.Trim('/')).TrimEnd('/');
        }

        private static string TryGetFirstHeader(this IHeaderDictionary headers, string name)
        {
            name = headers.Keys.FirstOrDefault(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase));

            if (name == null)
            {
                return null;
            }

#if AspNetOwin
            return headers[name].Split(',').Select(s => s.Trim()).First();
#else
            return headers[name].First().Split(',').Select(s => s.Trim()).First();
#endif
        }
    }
}
