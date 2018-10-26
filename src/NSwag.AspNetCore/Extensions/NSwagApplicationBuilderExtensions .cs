using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using NSwag.AspNetCore;
using NSwag.AspNetCore.Middlewares;
using NSwag.SwaggerGeneration.WebApi;
using System;
using System.Reflection;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>NSwag extensions for <see cref="IApplicationBuilder"/>.</summary>
    public static class NSwagApplicationBuilderExtensions
    {
        /// <summary>Adds the OpenAPI/Swagger generator that uses Api Description to perform Swagger generation.</summary>
        /// <remarks>This is the same as UseOpenApi(), configure the document in AddSwagger/AddOpenApi().</remarks>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure additional settings.</param>
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, Action<SwaggerMiddlewareSettings> configure = null)
        {
            return UseSwaggerWithApiExplorerCore(app, "v1", configure);
        }

        /// <summary>Adds the OpenAPI/Swagger generator that uses Api Description to perform Swagger generation.</summary>
        /// <remarks>This is the same as UseOpenApi(), configure the document in AddSwagger/AddOpenApi().</remarks>
        /// <param name="app">The app.</param>
        /// <param name="documentName">The document name (identifier from <see cref="NSwagServiceCollectionExtensions.AddSwagger"/>).</param>
        /// <param name="configure">Configure additional settings.</param>
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, string documentName, Action<SwaggerMiddlewareSettings> configure = null)
        {
            return UseSwaggerWithApiExplorerCore(app, documentName, configure);
        }

        /// <summary>Adds the Swagger UI (only) to the pipeline.</summary>
        /// <remarks>The settings.GeneratorSettings property does not have any effect.</remarks>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerUi3(
            this IApplicationBuilder app,
            Action<SwaggerUi3Settings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            var settings = new SwaggerUi3Settings<WebApiToSwaggerGeneratorSettings>();
            configure?.Invoke(settings);

            app.UseMiddleware<RedirectMiddleware>(settings.ActualSwaggerUiRoute, settings.ActualSwaggerRoute);
            app.UseMiddleware<SwaggerUiIndexMiddleware<WebApiToSwaggerGeneratorSettings>>(settings.ActualSwaggerUiRoute + "/index.html", settings, "NSwag.AspNetCore.SwaggerUi3.index.html");
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(settings.ActualSwaggerUiRoute),
                FileProvider = new EmbeddedFileProvider(typeof(SwaggerExtensions).GetTypeInfo().Assembly, "NSwag.AspNetCore.SwaggerUi3")
            });

            return app;
        }

        /// <summary>Adds the ReDoc UI (only) to the pipeline.</summary>
        /// <remarks>The settings.GeneratorSettings property does not have any effect.</remarks>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseReDoc(
            this IApplicationBuilder app,
            Action<SwaggerReDocSettings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            var settings = new SwaggerReDocSettings<WebApiToSwaggerGeneratorSettings>();
            configure?.Invoke(settings);

            app.UseMiddleware<RedirectMiddleware>(settings.ActualSwaggerUiRoute, settings.ActualSwaggerRoute);
            app.UseMiddleware<SwaggerUiIndexMiddleware<WebApiToSwaggerGeneratorSettings>>(settings.ActualSwaggerUiRoute + "/index.html", settings, "NSwag.AspNetCore.ReDoc.index.html");
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(settings.ActualSwaggerUiRoute),
                FileProvider = new EmbeddedFileProvider(typeof(SwaggerExtensions).GetTypeInfo().Assembly, "NSwag.AspNetCore.ReDoc")
            });

            return app;
        }

        private static IApplicationBuilder UseSwaggerWithApiExplorerCore(IApplicationBuilder app, string documentName, Action<SwaggerMiddlewareSettings> configure)
        {
            return app.UseMiddleware<SwaggerMiddleware>(documentName, configure ?? new Action<SwaggerMiddlewareSettings>((s) => { }));
        }
    }
}
