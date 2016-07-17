using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace NSwag.AspNet.Owin
{
    internal class SwaggerUiIndexMiddleware : OwinMiddleware
    {
        private readonly string _indexPath;
        private readonly SwaggerUiOwinSettings _settings;

        public SwaggerUiIndexMiddleware(OwinMiddleware next, string indexPath, SwaggerUiOwinSettings settings)
            : base(next)
        {
            _indexPath = indexPath;
            _settings = settings;
        }

        public override async Task Invoke(IOwinContext context)
        {
            var url = context.Request.Uri;
            if (url.PathAndQuery.Trim('/').StartsWith(_indexPath.Trim('/')))
            {
                var stream = typeof(SwaggerUiIndexMiddleware).Assembly.GetManifestResourceStream("NSwag.AspNet.Owin.SwaggerUi.index.html");
                using (var reader = new StreamReader(stream))
                {
                    var html = reader.ReadToEnd();

                    var oauth2Settings = _settings.OAuth2Client ?? new OAuth2Settings();
                    foreach (var property in oauth2Settings.GetType().GetRuntimeProperties())
                    {
                        var value = property.GetValue(oauth2Settings);
                        html = html.Replace("{" + property.Name + "}", value is IDictionary ? JsonConvert.SerializeObject(value) : value?.ToString() ?? "");
                    }

                    context.Response.StatusCode = 200;
                    context.Response.Write(html);
                }
            }
            else
                await Next.Invoke(context);
        }
    }
}