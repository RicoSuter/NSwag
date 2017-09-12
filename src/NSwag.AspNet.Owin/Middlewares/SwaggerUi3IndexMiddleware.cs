using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace NSwag.AspNet.Owin.Middlewares
{
    internal class SwaggerUi3IndexMiddleware : OwinMiddleware
    {
        private readonly string _indexPath;
        private readonly SwaggerUi3Settings _settings;

        public SwaggerUi3IndexMiddleware(OwinMiddleware next, string indexPath, SwaggerUi3Settings settings)
            : base(next)
        {
            _indexPath = indexPath;
            _settings = settings;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Path.HasValue && context.Request.Path.Value.Trim('/').StartsWith(_indexPath.Trim('/'), StringComparison.OrdinalIgnoreCase))
            {
                var stream = typeof(SwaggerUiIndexMiddleware).Assembly.GetManifestResourceStream("NSwag.AspNet.Owin.SwaggerUi3.index.html");
                using (var reader = new StreamReader(stream))
                {
                    var html = reader.ReadToEnd();

                    var oauth2Settings = _settings.OAuth2Client ?? new OAuth2ClientSettings();
                    foreach (var property in oauth2Settings.GetType().GetRuntimeProperties())
                    {
                        var value = property.GetValue(oauth2Settings);
                        html = html.Replace("{" + property.Name + "}", value is IDictionary ? JsonConvert.SerializeObject(value) : value?.ToString() ?? "");
                    }

                    html = html.Replace("{ValidatorUrl}", _settings.ValidateSpecification ? "undefined" : "null");
                    html = html.Replace("{DocExpansion}", _settings.DocExpansion);

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