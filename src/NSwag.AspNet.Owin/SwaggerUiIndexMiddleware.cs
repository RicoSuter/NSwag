using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;

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
            if (url.PathAndQuery.Trim('/') == _indexPath.Trim('/'))
            {
                var stream = typeof(SwaggerUiIndexMiddleware).Assembly.GetManifestResourceStream("NSwag.AspNet.Owin.SwaggerUi.index.html");
                using (var reader = new StreamReader(stream))
                {
                    var html = reader.ReadToEnd();

                    //foreach (var pair in _settings.SwaggerUiParameters)
                    //    html.Replace("{" + pair.Key + "}", pair.Value);

                    context.Response.StatusCode = 200;
                    context.Response.Write(html);
                }
            }
            else
                await Next.Invoke(context);
        }
    }
}