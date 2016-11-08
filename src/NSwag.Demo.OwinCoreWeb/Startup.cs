using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using NSwag.AspNetCore;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors.Security;

namespace NSwag.Demo.OwinCoreWeb
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
            app.UseSwaggerUi(typeof(Startup).GetTypeInfo().Assembly, new SwaggerUiOwinSettings
            {
                DefaultPropertyNameHandling = PropertyNameHandling.CamelCase,
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
                        Scopes = new Dictionary<string, string>
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
        }
    }
}
