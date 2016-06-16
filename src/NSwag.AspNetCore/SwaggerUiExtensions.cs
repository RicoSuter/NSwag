//-----------------------------------------------------------------------
// <copyright file="SwaggerUiExtensions.cs" company="NSwag">
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
    public static class SwaggerUiExtensions
    {
        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="settings">The Swagger generator settings.</param>
        /// <param name="swaggerUrl">The Swagger specification url.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwagger(
            this IApplicationBuilder app,
            Assembly webApiAssembly,
            WebApiToSwaggerGeneratorSettings settings,
            string swaggerUrl = "/swagger/v1/swagger.json")
        {
            return app.UseSwagger(new[] { webApiAssembly }, settings, swaggerUrl);
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssemblies">The Web API assemblies to search for controller types.</param>
        /// <param name="settings">The Swagger generator settings.</param>
        /// <param name="swaggerUrl">The Swagger specification url.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwagger(
            this IApplicationBuilder app,
            IEnumerable<Assembly> webApiAssemblies,
            WebApiToSwaggerGeneratorSettings settings,
            string swaggerUrl = "/swagger/v1/swagger.json")
        {
            var controllerTypes = webApiAssemblies.SelectMany(WebApiToSwaggerGenerator.GetControllerClasses);
            return app.UseSwagger(controllerTypes, settings, swaggerUrl);
        }

        /// <summary>Addes the Swagger generator to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="settings">The Swagger generator settings.</param>
        /// <param name="swaggerUrl">The Swagger specification url.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwagger(
            this IApplicationBuilder app,
            IEnumerable<Type> controllerTypes,
            WebApiToSwaggerGeneratorSettings settings,
            string swaggerUrl = "/swagger/v1/swagger.json")
        {
            app.UseMiddleware<SwaggerMiddleware>(swaggerUrl, controllerTypes, settings);
            return app;
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="settings">The Swagger generator settings.</param>
        /// <param name="baseRoute">The Swagger UI base route.</param>
        /// <param name="swaggerUrl">The Swagger specification url.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerUi(
            this IApplicationBuilder app,
            Assembly webApiAssembly,
            WebApiToSwaggerGeneratorSettings settings,
            string baseRoute = "/swagger/ui",
            string swaggerUrl = "/swagger/v1/swagger.json")
        {
            return app.UseSwaggerUi(new[] { webApiAssembly }, settings, baseRoute, swaggerUrl);
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssemblies">The Web API assemblies to search for controller types.</param>
        /// <param name="settings">The Swagger generator settings.</param>
        /// <param name="baseRoute">The Swagger UI base route.</param>
        /// <param name="swaggerUrl">The Swagger specification url.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerUi(
            this IApplicationBuilder app,
            IEnumerable<Assembly> webApiAssemblies,
            WebApiToSwaggerGeneratorSettings settings,
            string baseRoute = "/swagger/ui",
            string swaggerUrl = "/swagger/v1/swagger.json")
        {
            var controllerTypes = webApiAssemblies.SelectMany(WebApiToSwaggerGenerator.GetControllerClasses);
            return app.UseSwaggerUi(controllerTypes, settings, baseRoute, swaggerUrl);
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="settings">The Swagger generator settings.</param>
        /// <param name="baseRoute">The Swagger UI base route.</param>
        /// <param name="swaggerUrl">The Swagger specification url.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerUi(
            this IApplicationBuilder app,
            IEnumerable<Type> controllerTypes,
            WebApiToSwaggerGeneratorSettings settings,
            string baseRoute = "/swagger/ui",
            string swaggerUrl = "/swagger/v1/swagger.json")
        {
            app.UseMiddleware<RedirectMiddleware>(baseRoute, baseRoute + "/index.html?url=" + Uri.EscapeDataString(swaggerUrl));
            app.UseMiddleware<SwaggerMiddleware>(swaggerUrl, controllerTypes, settings);
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(baseRoute),
                FileProvider = new EmbeddedFileProvider(typeof(SwaggerUiExtensions).GetTypeInfo().Assembly, "NSwag.AspNetCore.SwaggerUi")
            });

            return app;
        }
    }
}
