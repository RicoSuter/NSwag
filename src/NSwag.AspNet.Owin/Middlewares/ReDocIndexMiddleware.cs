using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace NSwag.AspNet.Owin.Middlewares
{
    internal class ReDocIndexMiddleware : OwinMiddleware
    {
        private readonly string _indexPath;
        private readonly SwaggerReDocSettings _settings;

        public ReDocIndexMiddleware(OwinMiddleware next, string indexPath, SwaggerReDocSettings settings)
            : base(next)
        {
            _indexPath = indexPath;
            _settings = settings;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Path.HasValue && context.Request.Path.Value.Trim('/').StartsWith(_indexPath.Trim('/'), StringComparison.OrdinalIgnoreCase))
            {
                var stream = typeof(SwaggerUiIndexMiddleware).Assembly.GetManifestResourceStream("NSwag.AspNet.Owin.ReDoc.index.html");
                using (var reader = new StreamReader(stream))
                {
                    var html = reader.ReadToEnd();

                    context.Response.Headers["Content-Type"] = "text/html; charset=utf-8";
                    context.Response.StatusCode = 200;
                    context.Response.Write(html);
                }
            }
            else
                await Next.Invoke(context);
        }
    }
}