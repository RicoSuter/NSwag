//-----------------------------------------------------------------------
// <copyright file="SwaggerExtensions.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using NSwag.CodeGeneration.SwaggerGenerators;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;
using Owin;

namespace NSwag.AspNet.Owin
{
    /// <summary>Provides OWIN extensions to enable Swagger UI.</summary>
    public static class SwaggerExtensions
    {
        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="settings">The Swagger generator settings.</param>
        /// <returns>The app builder.</returns>
        public static IAppBuilder UseSwagger(
            this IAppBuilder app,
            Assembly webApiAssembly,
            SwaggerOwinSettings settings)
        {
            return app.UseSwagger(new[] { webApiAssembly }, settings);
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssemblies">The Web API assemblies to search for controller types.</param>
        /// <param name="settings">The Swagger generator settings.</param>
        /// <returns>The app builder.</returns>
        public static IAppBuilder UseSwagger(
            this IAppBuilder app,
            IEnumerable<Assembly> webApiAssemblies,
            SwaggerOwinSettings settings)
        {
            var controllerTypes = webApiAssemblies.SelectMany(WebApiToSwaggerGenerator.GetControllerClasses);
            return app.UseSwagger(controllerTypes, settings);
        }

        /// <summary>Addes the Swagger generator to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="settings">The Swagger generator settings.</param>
        /// <returns>The app builder.</returns>
        public static IAppBuilder UseSwagger(
            this IAppBuilder app,
            IEnumerable<Type> controllerTypes,
            SwaggerOwinSettings settings)
        {
            app.Use<SwaggerMiddleware>(settings.ActualSwaggerRoute, controllerTypes, settings);
            app.UseStageMarker(PipelineStage.MapHandler);
            return app;
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="settings">The Swagger generator settings.</param>
        /// <returns>The app builder.</returns>
        public static IAppBuilder UseSwaggerUi(
            this IAppBuilder app,
            Assembly webApiAssembly,
            SwaggerUiOwinSettings settings)
        {
            return app.UseSwaggerUi(new[] { webApiAssembly }, settings);
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssemblies">The Web API assemblies to search for controller types.</param>
        /// <param name="settings">The Swagger generator settings.</param>
        /// <returns>The app builder.</returns>
        public static IAppBuilder UseSwaggerUi(
            this IAppBuilder app,
            IEnumerable<Assembly> webApiAssemblies,
            SwaggerUiOwinSettings settings)
        {
            var controllerTypes = webApiAssemblies.SelectMany(WebApiToSwaggerGenerator.GetControllerClasses);
            return app.UseSwaggerUi(controllerTypes, settings, new SwaggerJsonSchemaGenerator(settings));
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="settings">The Swagger generator settings.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        /// <returns>The app builder.</returns>
        public static IAppBuilder UseSwaggerUi(
            this IAppBuilder app,
            IEnumerable<Type> controllerTypes,
            SwaggerUiOwinSettings settings,
            SwaggerJsonSchemaGenerator schemaGenerator)
        {
            app.Use<RedirectMiddleware>(settings.ActualSwaggerUiRoute, settings.ActualSwaggerRoute);
            app.Use<SwaggerMiddleware>(settings.ActualSwaggerRoute, controllerTypes, settings, schemaGenerator);
            app.Use<SwaggerUiIndexMiddleware>(settings.ActualSwaggerUiRoute + "/index.html", settings);
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(settings.ActualSwaggerUiRoute),
                FileSystem = new EmbeddedResourceFileSystem(typeof(SwaggerExtensions).Assembly, "NSwag.AspNet.Owin.SwaggerUi")
            });
            app.UseStageMarker(PipelineStage.MapHandler);
            return app;
        }
    }
}
