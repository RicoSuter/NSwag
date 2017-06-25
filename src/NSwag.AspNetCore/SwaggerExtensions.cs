//-----------------------------------------------------------------------
// <copyright file="SwaggerExtensions.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using NSwag.SwaggerGeneration;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.AspNetCore
{
    /// <summary>Provides OWIN extensions to enable Swagger UI.</summary>
    public static class SwaggerExtensions
    {
        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="settings">The Swagger generator settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwagger(
            this IApplicationBuilder app,
            Assembly webApiAssembly,
            SwaggerSettings settings)
        {
            return app.UseSwagger(new[] { webApiAssembly }, settings);
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssemblies">The Web API assemblies to search for controller types.</param>
        /// <param name="settings">The Swagger generator settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwagger(
            this IApplicationBuilder app,
            IEnumerable<Assembly> webApiAssemblies,
            SwaggerSettings settings)
        {
            var controllerTypes = webApiAssemblies.SelectMany(WebApiToSwaggerGenerator.GetControllerClasses);
            return app.UseSwagger(controllerTypes, settings);
        }

        /// <summary>Addes the Swagger generator to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="settings">The Swagger generator settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwagger(
            this IApplicationBuilder app,
            IEnumerable<Type> controllerTypes,
            SwaggerSettings settings)
        {
            return app.UseSwagger(controllerTypes, settings, new SwaggerJsonSchemaGenerator(settings));
        }

        /// <summary>Addes the Swagger generator to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="settings">The Swagger generator settings.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwagger(
            this IApplicationBuilder app,
            IEnumerable<Type> controllerTypes,
            SwaggerSettings settings,
            SwaggerJsonSchemaGenerator schemaGenerator)
        {
            app.UseMiddleware<SwaggerMiddleware>(settings.ActualSwaggerRoute, controllerTypes, settings, schemaGenerator);
            return app;
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="settings">The Swagger UI and generator settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerUi(
            this IApplicationBuilder app,
            Assembly webApiAssembly,
            SwaggerUiSettings settings)
        {
            return app.UseSwaggerUi(new[] { webApiAssembly }, settings);
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssemblies">The Web API assemblies to search for controller types.</param>
        /// <param name="settings">The Swagger UI and generator settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerUi(
            this IApplicationBuilder app,
            IEnumerable<Assembly> webApiAssemblies,
            SwaggerUiSettings settings)
        {
            var controllerTypes = webApiAssemblies.SelectMany(WebApiToSwaggerGenerator.GetControllerClasses);
            return app.UseSwaggerUi(controllerTypes, settings, new SwaggerJsonSchemaGenerator(settings));
        }

        /// <summary>Addes the Swagger UI (only) to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="settings">The Swagger UI settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerUi(
            this IApplicationBuilder app,
            SwaggerUiSettings settings)
        {
            return app.UseSwaggerUi(null, settings, null);
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="settings">The Swagger UI and generator settings.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerUi(
            this IApplicationBuilder app,
            IEnumerable<Type> controllerTypes,
            SwaggerUiSettings settings,
            SwaggerJsonSchemaGenerator schemaGenerator)
        {
            if (controllerTypes != null)
                app.UseMiddleware<SwaggerMiddleware>(settings.ActualSwaggerRoute, controllerTypes, settings, schemaGenerator);

            app.UseMiddleware<RedirectMiddleware>(settings.ActualSwaggerUiRoute, settings.ActualSwaggerRoute);
            app.UseMiddleware<SwaggerUiIndexMiddleware>(settings.ActualSwaggerUiRoute + "/index.html", settings);
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(settings.ActualSwaggerUiRoute),
                FileProvider = new EmbeddedFileProvider(typeof(SwaggerExtensions).GetTypeInfo().Assembly, "NSwag.AspNetCore.SwaggerUi")
            });

            return app;
        }

        /// <summary>Adds the Swagger generator to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="configure">Configure the Swagger generator settings.</param>
        public static IApplicationBuilder UseSwagger(
            this IApplicationBuilder app,
            IEnumerable<Type> controllerTypes,
            Action<SwaggerSettings> configure = null)
        {
            var settings = new SwaggerSettings();
            configure?.Invoke(settings);
            return app.UseSwagger(controllerTypes, settings, new SwaggerJsonSchemaGenerator(settings));
        }

        /// <summary>Adds the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="configure">Configure the Swagger generator and UI settings.</param>
        public static IApplicationBuilder UseSwaggerUi(
            this IApplicationBuilder app,
            IEnumerable<Type> controllerTypes,
            Action<SwaggerUiSettings> configure = null)
        {
            var settings = new SwaggerUiSettings();
            configure?.Invoke(settings);
            return app.UseSwaggerUi(controllerTypes, settings, new SwaggerJsonSchemaGenerator(settings));
        }
    }
}
