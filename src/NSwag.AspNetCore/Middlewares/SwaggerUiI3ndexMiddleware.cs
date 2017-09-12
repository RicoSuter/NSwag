using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace NSwag.AspNetCore.Middlewares
{
    internal class SwaggerUi3NdexMiddleware
    {
        private readonly RequestDelegate _nextDelegate;
        private readonly string _indexPath;
        private readonly SwaggerUi3Settings _settings;

        public SwaggerUi3NdexMiddleware(RequestDelegate nextDelegate, string indexPath, SwaggerUi3Settings settings)
        {
            _nextDelegate = nextDelegate;
            _indexPath = indexPath;
            _settings = settings;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.HasValue && context.Request.Path.Value.Trim('/').StartsWith(_indexPath.Trim('/'), StringComparison.OrdinalIgnoreCase))
            {
                var stream = typeof(SwaggerUiIndexMiddleware).GetTypeInfo().Assembly.GetManifestResourceStream("NSwag.AspNetCore.SwaggerUi3.index.html");
                using (var reader = new StreamReader(stream))
                {
                    var html = reader.ReadToEnd();

                    var oauth2Settings = _settings.OAuth2Client ?? new OAuth2ClientSettings();
                    foreach (var property in oauth2Settings.GetType().GetTypeInfo().GetProperties())
                    {
                        var value = property.GetValue(oauth2Settings);
                        html = html.Replace("{" + property.Name + "}", value is IDictionary ? JsonConvert.SerializeObject(value) : value?.ToString() ?? "");
                    }

                    html = html.Replace("{ValidatorUrl}", _settings.ValidateSpecification ? "undefined" : "null");
                    html = html.Replace("{DocExpansion}", _settings.DocExpansion);

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