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
    internal class RedirectMiddleware
    {
        private readonly RequestDelegate _nextDelegate;
        private readonly string _fromPath;
        private readonly string _swaggerPath;

        public RedirectMiddleware(RequestDelegate nextDelegate, string fromPath, string swaggerPath)
        {
            _nextDelegate = nextDelegate;
            _fromPath = fromPath;
            _swaggerPath = swaggerPath;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.HasValue &&
                string.Equals(context.Request.Path.Value.Trim('/'), _fromPath.Trim('/'), StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 302;

                if (context.Request.PathBase.HasValue)
                {
                    var suffix = !string.IsNullOrWhiteSpace(_swaggerPath) ? "?url=" + context.Request.PathBase.Value + _swaggerPath : "";
                    context.Response.Headers.Add("Location", context.Request.PathBase.Value + _fromPath + "/index.html" + suffix);
                }
                else
                {
                    var suffix = !string.IsNullOrWhiteSpace(_swaggerPath) ? "?url=" + _swaggerPath : "";
                    context.Response.Headers.Add("Location", _fromPath + "/index.html" + suffix);
                }
            }
            else
                await _nextDelegate.Invoke(context);
        }
    }
}
