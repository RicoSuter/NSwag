using System.Web.Http;
using Microsoft.Owin;
using NSwag.AspNet.Owin;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors;
using NSwag.Demo.OwinWeb;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace NSwag.Demo.OwinWeb
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            app.UseSwaggerUi(typeof(Startup).Assembly, new SwaggerUiOwinSettings
            {
                Title = "NSwag Sample API",
                OperationProcessors =
                {
                    new OAuth2OperationSecurityAppender()
                },
                DocumentProcessors =
                {
                    new OAuth2SchemeAppender("auth", new SwaggerSecurityScheme
                    {
                        Description = "Foo", 
                        Flow = "implicit", 
                        AuthorizationUrl = "https://localhost:44333/core/connect/authorize",
                        TokenUrl = "https://localhost:44333/core/connect/token", 
                        Scopes = 
                        {
                            { "read", "Read access to protected resources" },
                            { "write", "Write access to protected resources" }
                        }
                    })
                }
            });
            app.UseWebApi(config);

            config.MapHttpAttributeRoutes();
            config.EnsureInitialized();
        }
    }
}