using Microsoft.Owin;
using NSwag.Generation;

namespace NSwag.AspNet.Owin.Middlewares
{
    internal sealed class SwaggerUiIndexMiddleware<T> : OwinMiddleware
        where T : OpenApiDocumentGeneratorSettings, new()
    {
        private readonly string _indexPath;
        private readonly SwaggerUiSettings<T> _settings;
        private readonly string _resourcePath;

        public SwaggerUiIndexMiddleware(OwinMiddleware next, string indexPath, SwaggerUiSettings<T> settings, string resourcePath)
            : base(next)
        {
            _indexPath = indexPath;
            _settings = settings;
            _resourcePath = resourcePath;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Path.HasValue && string.Equals(context.Request.Path.Value.Trim('/'), _indexPath.Trim('/'), StringComparison.OrdinalIgnoreCase))
            {
                var stream = typeof(SwaggerUiIndexMiddleware<T>).Assembly.GetManifestResourceStream(_resourcePath);
                using var reader = new StreamReader(stream);
                context.Response.Headers["Content-Type"] = "text/html; charset=utf-8";
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync(await _settings.TransformHtmlAsync(reader.ReadToEnd(), context.Request, CancellationToken.None));
            }
            else
            {
                await Next.Invoke(context);
            }
        }
    }
}