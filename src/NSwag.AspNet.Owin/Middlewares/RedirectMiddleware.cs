//-----------------------------------------------------------------------
// <copyright file="RedirectMiddleware.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace NSwag.AspNet.Owin.Middlewares
{
    internal class RedirectMiddleware : OwinMiddleware
    {
        private readonly string _fromPath;
        private readonly string _swaggerPath;

        public RedirectMiddleware(OwinMiddleware next, string fromPath, string swaggerPath)
            : base(next)
        {
            _fromPath = fromPath;
            _swaggerPath = swaggerPath;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Path.HasValue &&
                string.Equals(context.Request.Path.Value.Trim('/'), _fromPath.Trim('/'), StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 302;

                if (context.Request.PathBase.HasValue)
                    context.Response.Headers.Set("Location", context.Request.PathBase.Value + _fromPath + "/index.html?url=" + context.Request.PathBase.Value + _swaggerPath);
                else
                    context.Response.Headers.Set("Location", _fromPath + "/index.html?url=" + _swaggerPath);
            }
            else
                await Next.Invoke(context);
        }
    }
}
