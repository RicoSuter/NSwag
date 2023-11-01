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
        /// <remarks>Registers multiple routes/documents if the settings.Path contains a '{documentName}' placeholder.</remarks>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure additional settings.</param>
        public static IApplicationBuilder UseOpenApi(this IApplicationBuilder app, Action<OpenApiDocumentMiddlewareSettings> configure = null)
        {
            var settings = configure == null ? app.ApplicationServices.GetService<IOptions<OpenApiDocumentMiddlewareSettings>>()?.Value : null ?? new OpenApiDocumentMiddlewareSettings();
            configure?.Invoke(settings);

            if (settings.Path.Contains("{documentName}"))
            {
                var documents = app.ApplicationServices.GetRequiredService<IEnumerable<OpenApiDocumentRegistration>>();
                foreach (var document in documents)
                {
                    app = app.UseMiddleware<OpenApiDocumentMiddleware>(document.DocumentName, settings.Path.Replace("{documentName}", document.DocumentName), settings);
                }

                return app;
            }
            else
            {
                return app.UseMiddleware<OpenApiDocumentMiddleware>(settings.DocumentName, settings.Path, settings);
            }
        }

        /// <summary>Adds the Swagger UI (UI only) to the pipeline (default route: /swagger).</summary>
        /// <remarks>The settings.GeneratorSettings property does not have any effect.</remarks>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerUi(
            this IApplicationBuilder app,
            Action<SwaggerUiSettings> configure = null)
        {
            var settings = configure == null ? app.ApplicationServices.GetService<IOptions<SwaggerUiSettings>>()?.Value : null ?? new SwaggerUiSettings();
            configure?.Invoke(settings);

            UseSwaggerUiWithDocumentNamePlaceholderExpanding(app, settings, (swaggerRoute, swaggerUiRoute) =>
            {
                app.UseMiddleware<RedirectToIndexMiddleware>(swaggerUiRoute, swaggerRoute, settings.TransformToExternalPath);
                app.UseMiddleware<SwaggerUiIndexMiddleware>(swaggerUiRoute + "/index.html", settings, "NSwag.AspNetCore.SwaggerUi.index.html");
                app.UseFileServer(new FileServerOptions
                {
                    RequestPath = new PathString(swaggerUiRoute),
                    FileProvider = new EmbeddedFileProvider(typeof(NSwagApplicationBuilderExtensions).GetTypeInfo().Assembly, "NSwag.AspNetCore.SwaggerUi")
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
            Action<ReDocSettings> configure = null)
        {
            var settings = configure == null ? app.ApplicationServices.GetService<IOptions<ReDocSettings>>()?.Value : null ?? new ReDocSettings();
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

        /// <summary>Adds a redirect to the Apimundo.com user interface to the pipeline (default route: /apimundo).</summary>
        /// <remarks>The settings.GeneratorSettings property does not have any effect.</remarks>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure the UI settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseApimundo(
            this IApplicationBuilder app,
            Action<ApimundoUiSettings> configure = null)
        {
            var settings = configure == null ? app.ApplicationServices.GetService<IOptions<ApimundoUiSettings>>()?.Value : null ?? new ApimundoUiSettings();
            settings.Path = "/apimundo";
            configure?.Invoke(settings);

            var path = settings.Path;
            var compareToIds = (string.IsNullOrEmpty(settings.CompareWith) ? "local:local:0:0:0:latest" : settings.CompareWith).Split(':');
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.HasValue &&
                    string.Equals(context.Request.Path.Value.Trim('/'), path.Trim('/'), StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.Headers["Content-Type"] = "text/html; charset=utf-8";
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync(@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <meta http-equiv=""X-UA-Compatible"" content=""ie=edge"">
    <title>NSwag to Apimundo</title>
</head>
<body>
<form id='myForm' method='post' action='" + settings.ApimundoUrl +
    @"/api/endpoints/local?organizationName=" + compareToIds[0] +
    @"&projectName=" + compareToIds[1] +
    @"&serviceId=" + compareToIds[2] +
    @"&endpointId=" + compareToIds[3] +
    @"&environmentId=" + compareToIds[4] +
    @"&documentId=" + (compareToIds[5] == "latest" ? "" : compareToIds[5]) + @"'>
<input type=""text"" name=""data"" id=""myData"" style=""display: none"" />
</form>
Please wait...
<script>
    fetch('" + settings.DocumentPath + @"').then(function (response) {
        return response.text();
    }, function() { alert(""Could not downlaod '" + settings.DocumentPath + @"'."") }).then(function (text) {
        var form = document.getElementById('myForm');
        var data = document.getElementById('myData');
        data.value = btoa(text);
        var key = 'upload_" + settings.ApimundoUrl + @"';
        if (window.localStorage.getItem(key) == 'ok' || 
            confirm(""Do you want to view the specification on '" + settings.ApimundoUrl + @"'?\nThis choice will stored and not asked again."")) {
            window.localStorage.setItem(key, 'ok');
            form.submit();
        }
    });
</script>
</body>
</html>
");
                }
                else
                {
                    await next();
                }
            });

            return app;
        }

        private static void UseSwaggerUiWithDocumentNamePlaceholderExpanding(IApplicationBuilder app,
            SwaggerUiSettingsBase settings,
            Action<string, string> register,
            Func<IEnumerable<OpenApiDocumentRegistration>, bool> registerMultiple)
        {
            if (settings.ActualSwaggerDocumentPath.Contains("{documentName}"))
            {
                var documents = app.ApplicationServices.GetRequiredService<IEnumerable<OpenApiDocumentRegistration>>();
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
