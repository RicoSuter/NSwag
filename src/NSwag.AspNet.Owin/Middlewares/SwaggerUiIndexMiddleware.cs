using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;
using NSwag.SwaggerGeneration;

namespace NSwag.AspNet.Owin.Middlewares
{
    internal class SwaggerUiIndexMiddleware<T> : OwinMiddleware
        where T : SwaggerGeneratorSettings, new()
    {
        private readonly string _indexPath;
        private readonly SwaggerUiSettingsBase<T> _settings;
        private readonly string _resourcePath;

        public SwaggerUiIndexMiddleware(OwinMiddleware next, string indexPath, SwaggerUiSettingsBase<T> settings, string resourcePath)
            : base(next)
        {
            _indexPath = indexPath;
            _settings = settings;
            _resourcePath = resourcePath;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Path.HasValue && context.Request.Path.Value.Trim('/').StartsWith(_indexPath.Trim('/'), StringComparison.OrdinalIgnoreCase))
            {
                var stream = typeof(SwaggerUiIndexMiddleware<T>).Assembly.GetManifestResourceStream(_resourcePath);
                using (var reader = new StreamReader(stream))
                {
                    context.Response.Headers["Content-Type"] = "text/html; charset=utf-8";
                    context.Response.StatusCode = 200;
                    context.Response.Write(_settings.TransformHtml(reader.ReadToEnd()));
                }
            }
            else
                await Next.Invoke(context);
        }
    }
}