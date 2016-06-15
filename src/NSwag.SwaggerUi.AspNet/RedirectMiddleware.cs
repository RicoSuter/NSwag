//-----------------------------------------------------------------------
// <copyright file="RedirectMiddleware.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.Owin;

namespace NSwag.SwaggerUi.AspNet
{
    internal class RedirectMiddleware : OwinMiddleware
    {
        private readonly string _fromPath;
        private readonly string _toPath;

        public RedirectMiddleware(OwinMiddleware next, string fromPath, string toPath) 
            : base(next)
        {
            _fromPath = fromPath;
            _toPath = toPath;
        }

        public override async Task Invoke(IOwinContext context)
        {
            var url = context.Request.Uri;
            if (url.PathAndQuery.Trim('/') == _fromPath.Trim('/'))
            {
                context.Response.StatusCode = 301;
                context.Response.Headers.Set("Location", _toPath);
            }
            else
                await Next.Invoke(context);
        }
    }
}
