//-----------------------------------------------------------------------
// <copyright file="SwaggerApplicationBuilderExtensions.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using NSwag.AspNetCore;
using NSwag.AspNetCore.Middlewares;
using NSwag.SwaggerGeneration.AspNetCore;
using NSwag.SwaggerGeneration.WebApi;
using OptionsCreator = Microsoft.Extensions.Options.Options;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods for <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class SwaggerApplicationBuilderExtensions
    {
        private static readonly Assembly _assembly = typeof(SwaggerApplicationBuilderExtensions).GetTypeInfo().Assembly;

        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSwagger(
            this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.UseMiddleware<SwaggerMiddleware>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSwagger(
            this IApplicationBuilder builder,
            SwaggerMiddlewareOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return builder.UseMiddleware<SwaggerMiddleware>(OptionsCreator.Create(options));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="uiRouteTemplate"></param>
        /// <param name="generationMiddlewareOptions"></param>
        /// <returns></returns>
        /// <remarks>
        /// <para>
        /// Reviewers: Could create a tiny <c>SwaggerUi3Options</c> class containing just the route template to allow
        /// configuration of this middleware in a <c>Startup.ConfigureServices</c> method. But, this is pretty usable
        /// as-is.
        /// </para>
        /// <para>
        /// Reviewers: Named to avoid ambiguity with
        /// <see cref="SwaggerExtensions.UseSwaggerReDoc(IApplicationBuilder, Action{SwaggerReDocSettings{WebApiToSwaggerGeneratorSettings}})"/>.
        /// Suggestions for a better name?
        /// </para>
        /// </remarks>
        public static IApplicationBuilder UseSwaggerReDocUi(
            this IApplicationBuilder builder,
            string uiRouteTemplate = "/swagger/{documentName}",
            SwaggerMiddlewareOptions generationMiddlewareOptions = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (uiRouteTemplate == null)
            {
                throw new ArgumentNullException(nameof(uiRouteTemplate));
            }

            var registry = builder.ApplicationServices.GetRequiredService<DocumentRegistry>();
            generationMiddlewareOptions = generationMiddlewareOptions ?? builder
                .ApplicationServices
                .GetRequiredService<IOptions<SwaggerMiddlewareOptions>>()
                .Value;

            // Though it appears *possible* to create a single middleware to handle all defined documents, would
            // duplicate the existing middleware types, adding TemplateMatcher to each, and would still require this
            // loop for DefaultFilesMiddleware and StaticFileMiddleware.
            foreach (var documentName in registry.DocumentNames)
            {
                var path = generationMiddlewareOptions.SwaggerRoute.Replace("{documentName}", documentName);

                // Unlike UseSwaggerReDocWithApiExplorer, use SwaggerReDocSettings and not SwaggerUiSettings.The
                // NSwag.AspNetCore.ReDoc.index.html resource contains no patterns for TransformHtml(...) replacement.
                //
                // Do not copy most properties over. The UI middleware does not use GeneratorSettings et cetera.
                var innerSettings = new SwaggerReDocSettings<AspNetCoreToSwaggerGeneratorSettings>
                {
                    SwaggerRoute = path,
                    SwaggerUiRoute = uiRouteTemplate.Replace("{documentName}", documentName),
                };

                builder.UseUiMiddleware("NSwag.AspNetCore.ReDoc", innerSettings);
            }

            return builder;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="uiRouteTemplate"></param>
        /// <param name="documentName"></param>
        /// <param name="generationMiddlewareOptions"></param>
        /// <returns></returns>
        /// <remarks>
        /// Reviewers: If users wish to add the UI middleware for a single document, this is the place.
        /// </remarks>
        public static IApplicationBuilder UseSwaggerReDocUi(
            this IApplicationBuilder builder,
            string documentName,
            string uiRouteTemplate,
            SwaggerMiddlewareOptions generationMiddlewareOptions = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (documentName == null)
            {
                throw new ArgumentNullException(nameof(documentName));
            }

            if (uiRouteTemplate == null)
            {
                throw new ArgumentNullException(nameof(uiRouteTemplate));
            }

            var registry = builder.ApplicationServices.GetRequiredService<DocumentRegistry>();
            if (!registry.DocumentNames.Contains(documentName))
            {
                throw new ArgumentException($"No registered document named '{documentName}'.", nameof(documentName));
            }

            generationMiddlewareOptions = generationMiddlewareOptions ?? builder
                .ApplicationServices
                .GetRequiredService<IOptions<SwaggerMiddlewareOptions>>()
                .Value;

            var path = generationMiddlewareOptions.SwaggerRoute.Replace("{documentName}", documentName);

            // Unlike UseSwaggerReDocWithApiExplorer, use SwaggerReDocSettings and not SwaggerUiSettings.The
            // NSwag.AspNetCore.ReDoc.index.html resource contains no patterns for TransformHtml(...) replacement.
            //
            // Do not copy most properties over. The UI middleware does not use GeneratorSettings et cetera.
            var innerSettings = new SwaggerReDocSettings<AspNetCoreToSwaggerGeneratorSettings>
            {
                SwaggerRoute = path,
                SwaggerUiRoute = uiRouteTemplate.Replace("{documentName}", documentName),
            };

            builder.UseUiMiddleware("NSwag.AspNetCore.ReDoc", innerSettings);

            return builder;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <param name="generationMiddlewareOptions"></param>
        /// <returns></returns>
        /// <remarks>
        /// Reviewers: Named to avoid ambiguity with
        /// <see cref="SwaggerExtensions.UseSwaggerUi(IApplicationBuilder, Action{SwaggerUiSettings{WebApiToSwaggerGeneratorSettings}})"/>
        /// and
        /// <see cref="SwaggerExtensions.UseSwaggerUi3(IApplicationBuilder, Action{SwaggerUi3Settings{WebApiToSwaggerGeneratorSettings}})"/>.
        /// Suggestions for a better name?
        /// </remarks>
        public static IApplicationBuilder UseSwaggerUi4(
            this IApplicationBuilder builder,
            SwaggerUi3Options options = null,
            SwaggerMiddlewareOptions generationMiddlewareOptions = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            options = options ?? builder
                .ApplicationServices
                .GetRequiredService<IOptions<SwaggerUi3Options>>()
                .Value;

            // Reviewers: This could be done in a new SwaggerUiIndexMiddleware constructor overload that accepts
            // SwaggerUi3Options. Or, because that might not work well with the generic, could create another similar
            // middleware. That decision could be made at any time because the middleware is internal.
            //
            // Do not copy generator properties over. The UI middleware does not use GeneratorSettings et cetera.
            var innerSettings = new SwaggerUi3Settings<AspNetCoreToSwaggerGeneratorSettings>
            {
                ApisSorter = options.ApisSorter,
                DefaultModelExpandDepth = options.DefaultModelExpandDepth,
                DefaultModelsExpandDepth = options.DefaultModelsExpandDepth,
                DocExpansion = options.DocExpansion,
                EnableTryItOut = options.EnableTryItOut,
                OAuth2Client = options.OAuth2Client,
                OperationsSorter = options.OperationsSorter,
                ServerUrl = options.ServerUrl,
                SwaggerUiRoute = options.UiRouteTemplate,
                TagSorter = options.TagSorter,
                ValidateSpecification = options.ValidateSpecification,
            };

            foreach (var route in options.AdditionalSwaggerRoutes)
            {
                innerSettings.SwaggerRoutes.Add(route);
            }

            var registry = builder.ApplicationServices.GetRequiredService<DocumentRegistry>();
            generationMiddlewareOptions = generationMiddlewareOptions ?? builder
                .ApplicationServices
                .GetRequiredService<IOptions<SwaggerMiddlewareOptions>>()
                .Value;

            foreach (var documentName in registry.DocumentNames)
            {
                var path = generationMiddlewareOptions.SwaggerRoute.Replace("{documentName}", documentName);
                innerSettings.SwaggerRoutes.Add(new SwaggerUi3Route(documentName, path));
            }

            return builder.UseUiMiddleware("NSwag.AspNetCore.SwaggerUi3", innerSettings);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="documentName"></param>
        /// <param name="options"></param>
        /// <param name="generationMiddlewareOptions"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSwaggerUi4(
            this IApplicationBuilder builder,
            string documentName,
            SwaggerUi3Options options = null,
            SwaggerMiddlewareOptions generationMiddlewareOptions = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (documentName == null)
            {
                throw new ArgumentNullException(nameof(documentName));
            }

            var registry = builder.ApplicationServices.GetRequiredService<DocumentRegistry>();
            if (!registry.DocumentNames.Contains(documentName))
            {
                throw new ArgumentException($"No registered document named '{documentName}'.", nameof(documentName));
            }

            options = options ?? builder
                .ApplicationServices
                .GetRequiredService<IOptions<SwaggerUi3Options>>()
                .Value;

            // Do not copy generator properties over. The UI middleware does not use GeneratorSettings et cetera.
            var innerSettings = new SwaggerUi3Settings<AspNetCoreToSwaggerGeneratorSettings>
            {
                ApisSorter = options.ApisSorter,
                DefaultModelExpandDepth = options.DefaultModelExpandDepth,
                DefaultModelsExpandDepth = options.DefaultModelsExpandDepth,
                DocExpansion = options.DocExpansion,
                EnableTryItOut = options.EnableTryItOut,
                OAuth2Client = options.OAuth2Client,
                OperationsSorter = options.OperationsSorter,
                ServerUrl = options.ServerUrl,
                SwaggerUiRoute = options.UiRouteTemplate,
                TagSorter = options.TagSorter,
                ValidateSpecification = options.ValidateSpecification,
            };

            foreach (var route in options.AdditionalSwaggerRoutes)
            {
                innerSettings.SwaggerRoutes.Add(route);
            }

            generationMiddlewareOptions = generationMiddlewareOptions ?? builder
                .ApplicationServices
                .GetRequiredService<IOptions<SwaggerMiddlewareOptions>>()
                .Value;

            var path = generationMiddlewareOptions.SwaggerRoute.Replace("{documentName}", documentName);
            innerSettings.SwaggerRoutes.Add(new SwaggerUi3Route(documentName, path));

            return builder.UseUiMiddleware("NSwag.AspNetCore.SwaggerUi3", innerSettings);
        }

        // Modeled after SwaggerExtensions.UseSwaggerReDocWithApiExplorer(...) and UseSwaggerUi3WithApiExplorer(...)
        // but does not add AspNetCoreToSwaggerMiddleware2 or other middleware for document generation.
        private static IApplicationBuilder UseUiMiddleware(
            this IApplicationBuilder builder,
            string resourceBase,
            SwaggerUiSettingsBase<AspNetCoreToSwaggerGeneratorSettings> settings)
        {
            builder.UseMiddleware<RedirectMiddleware>(settings.ActualSwaggerUiRoute, settings.ActualSwaggerRoute);
            builder.UseMiddleware<SwaggerUiIndexMiddleware<AspNetCoreToSwaggerGeneratorSettings>>(
                settings.ActualSwaggerUiRoute + "/index.html",
                settings,
                $"{resourceBase}.index.html");
            builder.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(settings.ActualSwaggerUiRoute),
                FileProvider = new EmbeddedFileProvider(_assembly, resourceBase),
            });

            return builder;
        }
    }
}
