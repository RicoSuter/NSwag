//-----------------------------------------------------------------------
// <copyright file="OpenApiExtensions.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.ApiDescription;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using NSwag.AspNetCore;
using NSwag.AspNetCore.DependencyInjection;
using NSwag.AspNetCore.Middlewares;
using NSwag.SwaggerGeneration.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Open API extension methods for <see cref="IServiceCollection"/> and <see cref="IOpenApiBuilder"/>.
    /// </summary>
    public static class OpenApiExtensions
    {
        private static readonly Assembly _assembly = typeof(OpenApiExtensions).GetTypeInfo().Assembly;

        /// <summary>
        /// Adds services required for Open API document generation to <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="configure">
        /// An <see cref="Action{T}"/> to configure the <see cref="OpenApiDocumentSettings"/>. If
        /// <see langword="null"/>, adds an Open API document with the default configuration and name "v1".
        /// </param>
        /// <returns>An <see cref="IOpenApiBuilder"/> instance.</returns>
        public static IOpenApiBuilder AddOpenApi(
            this IServiceCollection services,
            Action<OpenApiDocumentSettings> configure = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services
                .AddSwagger()
                .AddSingleton<DocumentRegistry>()
                .AddSingleton<NSwagDocumentProvider>()

                // Used by the Microsoft.Extensions.ApiDescription tool
                .AddSingleton<IDocumentProvider>(s => s.GetRequiredService<NSwagDocumentProvider>());

            var builder = new OpenApiBuilder(services);
            if (configure == null)
            {
                var settings = new OpenApiDocumentSettings();
                services.TryAddEnumerable(ServiceDescriptor.Singleton(settings));
            }
            else
            {
                builder.AddDocument(configure);
            }

            return builder;
        }

        /// <summary>
        /// Add an Open API document to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IOpenApiBuilder AddDocument(
            this IOpenApiBuilder builder,
            Action<OpenApiDocumentSettings> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var settings = new OpenApiDocumentSettings();
            configure(settings);
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton(settings));

            return builder;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseOpenApi(
            this IApplicationBuilder builder,
            Action<AspNetCoreToOpenApiMiddlewareSettings> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var settings = new AspNetCoreToOpenApiMiddlewareSettings();
            configure?.Invoke(settings);

            return builder.UseMiddleware<AspNetCoreToOpenApiMiddleware>(settings);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="uiRouteTemplate"></param>
        /// <param name="configureDocumentMiddleware"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseOpenApiWithReDoc(
            this IApplicationBuilder builder,
            string uiRouteTemplate = "",
            Action<AspNetCoreToOpenApiMiddlewareSettings> configureDocumentMiddleware = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var settings = new AspNetCoreToOpenApiMiddlewareSettings();
            configureDocumentMiddleware?.Invoke(settings);

            builder.UseMiddleware<AspNetCoreToOpenApiMiddleware>(settings);

            // Though it appears *possible* to create a single middleware to handle all defined documents, would
            // duplicate the existing middleware types, adding TemplateMatcher to each, and would still require this
            // loop for DefaultFilesMiddleware and StaticFileMiddleware.
            var registry = builder.ApplicationServices.GetRequiredService<DocumentRegistry>();
            foreach (var documentName in registry.DocumentNames)
            {
                // Unlike UseSwaggerReDocWithApiExplorer, use SwaggerReDocSettings and not SwaggerUiSettings.The
                // NSwag.AspNetCore.ReDoc.index.html resource contains no patterns for TransformHtml(...) replacement.
                //
                // Do not copy most properties over. The UI middleware does not use GeneratorSettings et cetera.
                var innerSettings = new SwaggerReDocSettings<AspNetCoreToSwaggerGeneratorSettings>
                {
                    SwaggerRoute = settings.SwaggerRoute.Replace("{documentName}", documentName),
                    SwaggerUiRoute = uiRouteTemplate.Replace("{documentName}", documentName),
                };

                builder.UseOpenApiWithUiCore("NSwag.AspNetCore.ReDoc", innerSettings);
            }

            return builder;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureUiMiddleware"></param>
        /// <param name="configureDocumentMiddleware"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseOpenApiWithUi3(
            this IApplicationBuilder builder,
            Action<OpenApiUi3Settings> configureUiMiddleware = null,
            Action<AspNetCoreToOpenApiMiddlewareSettings> configureDocumentMiddleware = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var settings = new AspNetCoreToOpenApiMiddlewareSettings();
            configureDocumentMiddleware?.Invoke(settings);

            builder.UseMiddleware<AspNetCoreToOpenApiMiddleware>(settings);

            var uiSettings = new OpenApiUi3Settings();
            configureUiMiddleware?.Invoke(uiSettings);

            // Do not copy generator properties over. The UI middleware does not use GeneratorSettings et cetera.
            var innerSettings = new SwaggerUi3Settings<AspNetCoreToSwaggerGeneratorSettings>
            {
                ApisSorter = uiSettings.ApisSorter,
                DefaultModelExpandDepth = uiSettings.DefaultModelExpandDepth,
                DefaultModelsExpandDepth = uiSettings.DefaultModelsExpandDepth,
                DocExpansion = uiSettings.DocExpansion,
                EnableTryItOut = uiSettings.EnableTryItOut,
                OAuth2Client = uiSettings.OAuth2Client,
                OperationsSorter = uiSettings.OperationsSorter,
                ServerUrl = uiSettings.ServerUrl,
                SwaggerUiRoute = uiSettings.UiRouteTemplate,
                TagSorter = uiSettings.TagSorter,
                ValidateSpecification = uiSettings.ValidateSpecification,
            };

            foreach (var route in uiSettings.AdditionalSwaggerRoutes)
            {
                innerSettings.SwaggerRoutes.Add(route);
            }

            var registry = builder.ApplicationServices.GetRequiredService<DocumentRegistry>();
            foreach (var documentName in registry.DocumentNames)
            {
                var path = settings.SwaggerRoute.Replace("{documentName}", documentName);
                innerSettings.SwaggerRoutes.Add(new SwaggerUi3Route(documentName, path));
            }

            return builder.UseOpenApiWithUiCore("NSwag.AspNetCore.SwaggerUi3", innerSettings);
        }

        // Modeled after SwaggerExtensions.UseSwaggerReDocWithApiExplorer(...) and UseSwaggerUi3WithApiExplorer(...)
        // but does not add AspNetCoreToOpenApiMiddleware or other middleware for document generation.
        private static IApplicationBuilder UseOpenApiWithUiCore(
            this IApplicationBuilder app,
            string resourceBase,
            SwaggerUiSettingsBase<AspNetCoreToSwaggerGeneratorSettings> settings)
        {
            app.UseMiddleware<RedirectMiddleware>(settings.ActualSwaggerUiRoute, settings.ActualSwaggerRoute);
            app.UseMiddleware<SwaggerUiIndexMiddleware<AspNetCoreToSwaggerGeneratorSettings>>(
                settings.ActualSwaggerUiRoute + "/index.html",
                settings,
                $"{resourceBase}.index.html");
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(settings.ActualSwaggerUiRoute),
                FileProvider = new EmbeddedFileProvider(_assembly, resourceBase),
            });

            return app;
        }
    }
}
