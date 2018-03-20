using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NSwag.SwaggerGeneration;

namespace NSwag.AspNetCore.Middlewares
{
    internal class SwaggerUiIndexMiddleware<T>
        where T : SwaggerGeneratorSettings, new()
    {
        private readonly RequestDelegate _nextDelegate;
        private readonly string _indexPath;
        private readonly SwaggerUiSettingsBase<T> _settings;
        private readonly string _resourcePath;

        public SwaggerUiIndexMiddleware(RequestDelegate nextDelegate, string indexPath, SwaggerUiSettingsBase<T> settings, string resourcePath)
        {
            _nextDelegate = nextDelegate;
            _indexPath = indexPath;
            _settings = settings;
            _resourcePath = resourcePath;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.HasValue && context.Request.Path.Value.Trim('/').StartsWith(_indexPath.Trim('/'), StringComparison.OrdinalIgnoreCase))
            {
                var stream = typeof(SwaggerUiIndexMiddleware<T>).GetTypeInfo().Assembly.GetManifestResourceStream(_resourcePath);
                using (var reader = new StreamReader(stream))
                {
                    context.Response.Headers["Content-Type"] = "text/html; charset=utf-8";
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync(_settings.TransformHtml(await reader.ReadToEndAsync()));
                }
            }
            else
                await _nextDelegate(context);
        }
    }
}