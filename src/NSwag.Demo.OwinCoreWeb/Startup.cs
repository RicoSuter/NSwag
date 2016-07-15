using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSwag.AspNetCore;
using NSwag.Demo.OwinCoreWeb.Controllers;
using NJsonSchema.Infrastructure;

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
                Title = "NSwag Sample API",
                OAuth2 = new OAuth2Settings
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
                //OperationProcessors =
                //{
                //    new OAuth2OperationSecurityAppender()
                //},
                //DocumentProcessors =
                //{
                //    new OAuth2SchemeAppender("auth", new SwaggerSecurityScheme
                //    {
                //        Description = "Foo",
                //        Flow = "implicit",
                //        AuthorizationUrl = "https://localhost:44333/core/connect/authorize",
                //        TokenUrl = "https://localhost:44333/core/connect/token",
                //        Scopes =
                //        {
                //            { "read", "Read access to protected resources" },
                //            { "write", "Write access to protected resources" }
                //        }
                //    })
                //}
            });
        }
    }
}
