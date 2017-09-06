using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NSwag.AspNetCore
{
    internal class ReDocIndexMiddleware
    {
        private readonly RequestDelegate _nextDelegate;
        private readonly string _indexPath;
        private readonly SwaggerUiSettings _settings;

        public ReDocIndexMiddleware(RequestDelegate nextDelegate, string indexPath, SwaggerUiSettings settings)
        {
            _nextDelegate = nextDelegate;
            _indexPath = indexPath;
            _settings = settings;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.HasValue && context.Request.Path.Value.Trim('/').StartsWith(_indexPath.Trim('/'), StringComparison.OrdinalIgnoreCase))
            {
                var stream = typeof(SwaggerUiIndexMiddleware).GetTypeInfo().Assembly.GetManifestResourceStream("NSwag.AspNetCore.SwaggerUi.index.html");
                using (var reader = new StreamReader(stream))
                {
                    var html = reader.ReadToEnd();

                    context.Response.Headers["Content-Type"] = "text/html; charset=utf-8";
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync(html);
                }
            }
            else
                await _nextDelegate(context);
        }
    }
}