using System.Collections.Generic;
using System.Web.Http;
using Microsoft.Owin;
using NSwag.AspNet.Owin;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors.Security;
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
                OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = "foo",
                    ClientSecret = "bar",
                    AppName = "my_app",
                    Realm = "my_realm",
                    AdditionalQueryStringParameters =
                    {
                        { "foo", "bar" }
                    }
                },
                OperationProcessors =
                {
                    new OperationSecurityScopeProcessor("oauth2")
                },
                DocumentProcessors =
                {
                    new SecurityDefinitionAppender("oauth2", new SwaggerSecurityScheme
                    {
                        Type = SwaggerSecuritySchemeType.OAuth2,
                        Description = "Foo",
                        Flow = SwaggerOAuth2Flow.Implicit,
                        AuthorizationUrl = "https://localhost:44333/core/connect/authorize",
                        TokenUrl = "https://localhost:44333/core/connect/token",
                        Scopes = new Dictionary<string,string>
                        {
                            { "read", "Read access to protected resources" },
                            { "write", "Write access to protected resources" }
                        }
                    }),
                    new SecurityDefinitionAppender("apikey", new SwaggerSecurityScheme
                    {
                        Type = SwaggerSecuritySchemeType.ApiKey,
                        Name = "api_key",
                        In = SwaggerSecurityApiKeyLocation.Header
                    })
                }
            });
            app.UseWebApi(config);

            config.MapHttpAttributeRoutes();
            config.EnsureInitialized();
        }
    }
}