//-----------------------------------------------------------------------
// <copyright file="RedirectMiddleware.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NSwag.AspNetCore.Middlewares
{
    internal class RedirectToIndexMiddleware
    {
        private readonly RequestDelegate _nextDelegate;

        private readonly string _swaggerUiRoute;
        private readonly string _swaggerRoute;

        private readonly Func<string, HttpRequest, string> _transformToExternal;

        public RedirectToIndexMiddleware(RequestDelegate nextDelegate, string internalSwaggerUiRoute, string internalSwaggerRoute, Func<string, HttpRequest, string> transformToExternal)
        {
            _nextDelegate = nextDelegate;

            _swaggerUiRoute = internalSwaggerUiRoute;
            _swaggerRoute = internalSwaggerRoute;

            _transformToExternal = transformToExternal;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.HasValue &&
                string.Equals(context.Request.Path.Value.Trim('/'), _swaggerUiRoute.Trim('/'), StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 302;

                if (context.Request.PathBase.HasValue)
                {
                    var suffix = !string.IsNullOrWhiteSpace(_swaggerRoute) ?
                        "?url=" + _transformToExternal(context.Request.PathBase.Value + _swaggerRoute, context.Request) :
                        "";

                    context.Response.Headers.Add("Location",
                        _transformToExternal(context.Request.PathBase.Value + _swaggerUiRoute, context.Request) + "/index.html" + suffix);
                }
                else
                {
                    var suffix = !string.IsNullOrWhiteSpace(_swaggerRoute) ? "?url=" + _transformToExternal(_swaggerRoute, context.Request) : "";
                    context.Response.Headers.Add("Location", _transformToExternal(_swaggerUiRoute, context.Request) + "/index.html" + suffix);
                }
            }
            else
                await _nextDelegate.Invoke(context);
        }
    }
}
