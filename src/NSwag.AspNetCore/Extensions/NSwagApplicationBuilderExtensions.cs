using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using NSwag.AspNetCore;
using NSwag.AspNetCore.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>NSwag extensions for <see cref="IApplicationBuilder"/>.</summary>
    public static class NSwagApplicationBuilderExtensions
    {
        /// <summary>Adds the OpenAPI/Swagger generator that uses the ASP.NET Core API Explorer 
        /// (default route defined in document: /swagger/v1/swagger.json).</summary>
        /// <remarks>Registers multiple routes/documents if the settings.Path contains a '{documentName}' placeholder.
        /// The methods <see cref="UseOpenApi"/> and <see cref="UseSwagger"/> are the same, but <see cref="UseSwagger"/> will be deprecated eventually.</remarks>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure additional settings.</param>
        public static IApplicationBuilder UseOpenApi(this IApplicationBuilder app, Action<SwaggerDocumentMiddlewareSettings> configure = null)
        {
            return UseSwaggerWithApiExplorerCore(app, configure);
        }

        /// <summary>Adds the OpenAPI/Swagger generator that uses the ASP.NET Core API Explorer 
        /// (default route defined in document: /swagger/v1/swagger.json).</summary>
        /// <remarks>Registers multiple routes/documents if the settings.Path contains a '{documentName}' placeholder.
        /// The methods <see cref="UseOpenApi"/> and <see cref="UseSwagger"/> are the same, but <see cref="UseSwagger"/> will be deprecated eventually.</remarks>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure additional settings.</param>
        [Obsolete("Use UseOpenApi() instead.")]
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, Action<SwaggerDocumentMiddlewareSettings> configure = null)
        {
            return UseSwaggerWithApiExplorerCore(app, configure);
        }

        private static IApplicationBuilder UseSwaggerWithApiExplorerCore(IApplicationBuilder app, Action<SwaggerDocumentMiddlewareSettings> configure)
        {
            var settings = configure == null ? app.ApplicationServices.GetService<IOptions<SwaggerDocumentMiddlewareSettings>>()?.Value : null ?? new SwaggerDocumentMiddlewareSettings();
            configure?.Invoke(settings);

            if (settings.Path.Contains("{documentName}"))
            {
                var documents = app.ApplicationServices.GetRequiredService<IEnumerable<SwaggerDocumentRegistration>>();
                foreach (var document in documents)
                {
                    app = app.UseMiddleware<SwaggerDocumentMiddleware>(document.DocumentName, settings.Path.Replace("{documentName}", document.DocumentName), settings);
                }

                return app;
            }
            else
            {
                return app.UseMiddleware<SwaggerDocumentMiddleware>(settings.DocumentName, settings.Path, settings);
            }
        }

        /// <summary>Adds the Swagger UI (UI only) to the pipeline (default route: /swagger).</summary>
        /// <remarks>The settings.GeneratorSettings property does not have any effect.</remarks>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerUi3(
            this IApplicationBuilder app,
            Action<SwaggerUi3Settings> configure = null)
        {
            var settings = configure == null ? app.ApplicationServices.GetService<IOptions<SwaggerUi3Settings>>()?.Value : null ?? new SwaggerUi3Settings();
            configure?.Invoke(settings);

            UseSwaggerUiWithDocumentNamePlaceholderExpanding(app, settings, (swaggerRoute, swaggerUiRoute) =>
            {
                app.UseMiddleware<RedirectToIndexMiddleware>(swaggerUiRoute, swaggerRoute, settings.TransformToExternalPath);
                app.UseMiddleware<SwaggerUiIndexMiddleware>(swaggerUiRoute + "/index.html", settings, "NSwag.AspNetCore.SwaggerUi3.index.html");
                app.UseFileServer(new FileServerOptions
                {
                    RequestPath = new PathString(swaggerUiRoute),
                    FileProvider = new EmbeddedFileProvider(typeof(NSwagApplicationBuilderExtensions).GetTypeInfo().Assembly, "NSwag.AspNetCore.SwaggerUi3")
                });
            },
            (documents) =>
            {
                var swaggerRouteWithPlaceholder = settings.ActualSwaggerDocumentPath;

                settings.SwaggerRoutes.Clear();
                foreach (var document in documents)
                {
                    var swaggerRoute = swaggerRouteWithPlaceholder.Replace("{documentName}", document.DocumentName);
                    settings.SwaggerRoutes.Add(new SwaggerUi3Route(document.DocumentName, swaggerRoute));
                }

                return true;
            });

            return app;
        }

        /// <summary>Adds the ReDoc UI (UI only) to the pipeline (default route: /swagger).</summary>
        /// <remarks>The settings.GeneratorSettings property does not have any effect.</remarks>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseReDoc(
            this IApplicationBuilder app,
            Action<SwaggerReDocSettings> configure = null)
        {
            var settings = configure == null ? app.ApplicationServices.GetService<IOptions<SwaggerReDocSettings>>()?.Value : null ?? new SwaggerReDocSettings();
            configure?.Invoke(settings);

            UseSwaggerUiWithDocumentNamePlaceholderExpanding(app, settings, (swaggerRoute, swaggerUiRoute) =>
            {
                app.UseMiddleware<RedirectToIndexMiddleware>(swaggerUiRoute, swaggerRoute, settings.TransformToExternalPath);
                app.UseMiddleware<SwaggerUiIndexMiddleware>(swaggerUiRoute + "/index.html", settings, "NSwag.AspNetCore.ReDoc.index.html");
                app.UseFileServer(new FileServerOptions
                {
                    RequestPath = new PathString(swaggerUiRoute),
                    FileProvider = new EmbeddedFileProvider(typeof(NSwagApplicationBuilderExtensions).GetTypeInfo().Assembly, "NSwag.AspNetCore.ReDoc")
                });
            }, (documents) => false);

            return app;
        }

        /// <summary>Adds the Swagger UI (only) to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure the Swagger UI settings.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(UseSwaggerUi3) + " instead.")]
        public static IApplicationBuilder UseSwaggerUi(
            this IApplicationBuilder app,
            Action<SwaggerUiSettings> configure = null)
        {
            var settings = new SwaggerUiSettings();
            settings.DocumentPath = "/swagger/v1/swagger.json";
            configure?.Invoke(settings);

            app.UseMiddleware<RedirectToIndexMiddleware>(settings.ActualSwaggerUiPath, settings.ActualSwaggerDocumentPath, settings.TransformToExternalPath);
            app.UseMiddleware<SwaggerUiIndexMiddleware>(settings.ActualSwaggerUiPath + "/index.html", settings, "NSwag.AspNetCore.SwaggerUi.index.html");
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(settings.ActualSwaggerUiPath),
                FileProvider = new EmbeddedFileProvider(typeof(NSwagApplicationBuilderExtensions).GetTypeInfo().Assembly, "NSwag.AspNetCore.SwaggerUi")
            });

            return app;
        }

        private static void UseSwaggerUiWithDocumentNamePlaceholderExpanding(IApplicationBuilder app,
            SwaggerUiSettingsBase settings,
            Action<string, string> register,
            Func<IEnumerable<SwaggerDocumentRegistration>, bool> registerMultiple)
        {
            if (settings.ActualSwaggerDocumentPath.Contains("{documentName}"))
            {
                var documents = app.ApplicationServices.GetRequiredService<IEnumerable<SwaggerDocumentRegistration>>();
                if (settings.ActualSwaggerUiPath.Contains("{documentName}"))
                {
                    // Register multiple uis for each document
                    foreach (var document in documents)
                    {
                        register(
                            settings.ActualSwaggerDocumentPath.Replace("{documentName}", document.DocumentName),
                            settings.ActualSwaggerUiPath.Replace("{documentName}", document.DocumentName));
                    }
                }
                else
                {
                    // Register single ui with multiple documents
                    if (registerMultiple(documents))
                    {
                        register(settings.ActualSwaggerDocumentPath, settings.ActualSwaggerUiPath);
                    }
                    else
                    {
                        // If multiple documents is not supported and only one document is registered
                        if (documents.Count() == 1)
                        {
                            register(
                                settings.ActualSwaggerDocumentPath.Replace("{documentName}", documents.First().DocumentName),
                                settings.ActualSwaggerUiPath.Replace("{documentName}", documents.First().DocumentName));
                        }
                        else
                        {
                            throw new NotSupportedException("This UI does not support multiple documents per UI: " +
                                "Do not use '{documentName}' placeholder in DocumentPath or Path.");
                        }
                    }
                }
            }
            else
            {
                if (settings.ActualSwaggerUiPath.Contains("{documentName}"))
                {
                    throw new ArgumentException("The SwaggerUiRoute cannot contain '{documentName}' placeholder when SwaggerRoute is missing the placeholder.");
                }

                // Register single ui with one document
                register(settings.ActualSwaggerDocumentPath, settings.ActualSwaggerUiPath);
            }
        }
    }
}
