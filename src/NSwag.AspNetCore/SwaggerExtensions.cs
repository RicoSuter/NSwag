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
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

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
            SwaggerOwinSettings settings)
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
        public static IApplicationBuilder UseSwagger(
            this IApplicationBuilder app,
            IEnumerable<Type> controllerTypes,
            SwaggerOwinSettings settings)
        {
            app.UseMiddleware<SwaggerMiddleware>(settings.SwaggerRoute, controllerTypes, settings);
            return app;
        }



        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="settings">The Swagger generator settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerUi(
            this IApplicationBuilder app,
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
        public static IApplicationBuilder UseSwaggerUi(
            this IApplicationBuilder app,
            IEnumerable<Assembly> webApiAssemblies,
            SwaggerUiOwinSettings settings)
        {
            var controllerTypes = webApiAssemblies.SelectMany(WebApiToSwaggerGenerator.GetControllerClasses);
            return app.UseSwaggerUi(controllerTypes, settings);
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="settings">The Swagger generator settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerUi(
            this IApplicationBuilder app,
            IEnumerable<Type> controllerTypes,
            SwaggerUiOwinSettings settings)
        {
            app.UseMiddleware<RedirectMiddleware>(settings.SwaggerUiRoute, settings.SwaggerUiRoute + "/index.html?url=" + Uri.EscapeDataString(settings.SwaggerRoute));
            app.UseMiddleware<SwaggerMiddleware>(settings.SwaggerRoute, controllerTypes, settings);
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(settings.SwaggerUiRoute),
                FileProvider = new EmbeddedFileProvider(typeof(SwaggerExtensions).GetTypeInfo().Assembly, "NSwag.AspNetCore.SwaggerUi")
            });

            return app;
        }
    }
}
