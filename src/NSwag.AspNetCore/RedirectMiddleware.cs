//-----------------------------------------------------------------------
// <copyright file="RedirectMiddleware.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NSwag.AspNetCore
{
    internal class RedirectMiddleware
    {
        private readonly RequestDelegate _nextDelegate;
        private readonly string _fromPath;
        private readonly string _toPath;

        public RedirectMiddleware(RequestDelegate nextDelegate, string fromPath, string toPath)
        {
            _nextDelegate = nextDelegate;
            _fromPath = fromPath;
            _toPath = toPath;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.Trim('/') == _fromPath.Trim('/'))
            {
                context.Response.StatusCode = 301;
                context.Response.Headers.Add("Location", _toPath);
            }
            else
                await _nextDelegate.Invoke(context);
        }
    }
}
