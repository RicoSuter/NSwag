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
        {
            return request.Headers["X-Forwarded-Proto"] ?? request.Scheme;
#else
        private static string GetHttpScheme(this HttpRequest request)
        {
            return request.Headers["X-Forwarded-Proto"].FirstOrDefault() ?? request.Scheme;
#endif
        }

#if AspNetOwin
        public static string GetServerUrl(this IOwinRequest request)
        {
            var baseUrl = request.Headers.ContainsKey("X-Forwarded-Host") ?
                new Uri($"{request.GetHttpScheme()}://{request.Headers["X-Forwarded-Host"]}").ToString().TrimEnd('/') :
                new Uri($"{request.GetHttpScheme()}://{request.Host}").ToString().TrimEnd('/');
#else
        public static string GetServerUrl(this HttpRequest request)
        {
            var baseUrl = request.Headers.ContainsKey("X-Forwarded-Host") ?
                new Uri($"{request.GetHttpScheme()}://{request.Headers["X-Forwarded-Host"].First()}").ToString().TrimEnd('/') :
                new Uri($"{request.GetHttpScheme()}://{request.Host}").ToString().TrimEnd('/');
#endif

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
#if AspNetOwin
                return "/" + request.Headers["X-Forwarded-Prefix"].Trim('/');
#else
                return "/" + request.Headers["X-Forwarded-Prefix"].First().Trim('/');
#endif
            }

            var basePath = request.Headers.ContainsKey("X-Forwarded-Host") ?
                new Uri($"http://{request.Headers["X-Forwarded-Host"].First()}").AbsolutePath :
                "";

            if (request.PathBase.HasValue)
            {
                basePath = basePath.TrimEnd('/') + "/" + request.PathBase.Value;
            }

            return ("/" + basePath.Trim('/')).TrimEnd('/');
        }
    }
}
