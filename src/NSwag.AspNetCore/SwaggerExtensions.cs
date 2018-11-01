//-----------------------------------------------------------------------
// <copyright file="SwaggerExtensions.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using NSwag.AspNetCore.Middlewares;
using NSwag.SwaggerGeneration;
using NSwag.SwaggerGeneration.AspNetCore;
using NSwag.SwaggerGeneration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSwag.AspNetCore
{
    // Obsolete extension methods only, see /Extensions directory for supported extensions.

    /// <summary>Provides extensions to enable Swagger UI.</summary>
    public static class SwaggerExtensions
    {
        #region Swagger

        /// <summary>Adds the Swagger generator that uses reflection (legacy) and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(NSwagServiceCollectionExtensions.AddSwagger) + "() and " + nameof(UseSwagger) + "() instead.")]
        public static IApplicationBuilder UseSwagger(
            this IApplicationBuilder app,
            Assembly webApiAssembly,
            Action<SwaggerSettings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            return app.UseSwagger(new[] { webApiAssembly }, configure);
        }

        /// <summary>Adds the Swagger generator that uses reflection (legacy) and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssemblies">The Web API assemblies to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(NSwagServiceCollectionExtensions.AddSwagger) + "() and " + nameof(UseSwagger) + "() instead.")]
        public static IApplicationBuilder UseSwagger(
            this IApplicationBuilder app,
            IEnumerable<Assembly> webApiAssemblies,
            Action<SwaggerSettings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            var controllerTypes = webApiAssemblies.SelectMany(WebApiToSwaggerGenerator.GetControllerClasses);
            return app.UseSwagger(controllerTypes, configure);
        }

        /// <summary>Adds the Swagger generator that uses reflection (legacy) to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(NSwagServiceCollectionExtensions.AddSwagger) + "() and " + nameof(UseSwagger) + "() instead.")]
        public static IApplicationBuilder UseSwagger(
            this IApplicationBuilder app,
            IEnumerable<Type> controllerTypes,
            Action<SwaggerSettings<WebApiToSwaggerGeneratorSettings>> configure = null,
            SwaggerJsonSchemaGenerator schemaGenerator = null)
        {
            var settings = new SwaggerSettings<WebApiToSwaggerGeneratorSettings>();
            configure?.Invoke(settings);
            app.UseMiddleware<WebApiToSwaggerMiddleware>(settings.ActualSwaggerRoute, controllerTypes, settings, schemaGenerator ?? new SwaggerJsonSchemaGenerator(settings.GeneratorSettings));
            return app;
        }

        /// <summary>Adds the Swagger generator that uses Api Description to perform Swagger generation.</summary>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure additional settings.</param>
        [Obsolete("Use " + nameof(UseSwagger) + " instead.")]
        public static IApplicationBuilder UseSwaggerWithApiExplorer(this IApplicationBuilder app, Action<SwaggerMiddlewareSettings> configure = null)
        {
            return NSwagApplicationBuilderExtensions.UseSwagger(app, configure);
        }

        #endregion

        #region SwaggerUi

        /// <summary>Adds the Swagger generator and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(NSwagServiceCollectionExtensions.AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerUi3) + "() instead.")]
        public static IApplicationBuilder UseSwaggerUi(
            this IApplicationBuilder app,
            Assembly webApiAssembly,
            Action<SwaggerUiSettings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            return app.UseSwaggerUi(new[] { webApiAssembly }, configure);
        }

        /// <summary>Adds the Swagger generator and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssemblies">The Web API assemblies to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(NSwagServiceCollectionExtensions.AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerUi3) + "() instead.")]
        public static IApplicationBuilder UseSwaggerUi(
            this IApplicationBuilder app,
            IEnumerable<Assembly> webApiAssemblies,
            Action<SwaggerUiSettings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            var controllerTypes = webApiAssemblies.SelectMany(WebApiToSwaggerGenerator.GetControllerClasses);
            return app.UseSwaggerUi(controllerTypes, configure);
        }

        /// <summary>Adds the Swagger UI (only) to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(UseSwaggerUi3) + " instead.")]
        public static IApplicationBuilder UseSwaggerUi(
            this IApplicationBuilder app,
            Action<SwaggerUiSettings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            return app.UseSwaggerUi(null, configure, null);
        }

        /// <summary>Adds the Swagger generator and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(NSwagServiceCollectionExtensions.AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerUi3) + "() instead.")]
        public static IApplicationBuilder UseSwaggerUi(
            this IApplicationBuilder app,
            IEnumerable<Type> controllerTypes,
            Action<SwaggerUiSettings<WebApiToSwaggerGeneratorSettings>> configure = null,
            SwaggerJsonSchemaGenerator schemaGenerator = null)
        {
            var settings = new SwaggerUiSettings<WebApiToSwaggerGeneratorSettings>();
            configure?.Invoke(settings);

            if (controllerTypes != null)
                app.UseMiddleware<WebApiToSwaggerMiddleware>(settings.ActualSwaggerRoute, controllerTypes, settings, schemaGenerator ?? new SwaggerJsonSchemaGenerator(settings.GeneratorSettings));

            app.UseMiddleware<RedirectMiddleware>(settings.ActualSwaggerUiRoute, settings.ActualSwaggerRoute);
            app.UseMiddleware<SwaggerUiIndexMiddleware<WebApiToSwaggerGeneratorSettings>>(settings.ActualSwaggerUiRoute + "/index.html", settings, "NSwag.AspNetCore.SwaggerUi.index.html");
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(settings.ActualSwaggerUiRoute),
                FileProvider = new EmbeddedFileProvider(typeof(SwaggerExtensions).GetTypeInfo().Assembly, "NSwag.AspNetCore.SwaggerUi")
            });

            return app;
        }

        /// <summary>Adds the Swagger generator and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(NSwagServiceCollectionExtensions.AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerUi3) + "() instead.")]
        public static IApplicationBuilder UseSwaggerUiWithApiExplorer(
            this IApplicationBuilder app,
            Action<SwaggerUiSettings<AspNetCoreToSwaggerGeneratorSettings>> configure = null,
            SwaggerJsonSchemaGenerator schemaGenerator = null)
        {
            throw new NotSupportedException("Use " + nameof(NSwagServiceCollectionExtensions.AddSwagger) + "(), " + nameof(UseSwagger) +
                "() and " + nameof(UseSwaggerUi3) + "() instead.");
        }

        #endregion

        #region SwaggerUi3

        /// <summary>Adds the Swagger generator that uses reflection (legacy) and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(NSwagServiceCollectionExtensions.AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerUi3) + "() instead.")]
        public static IApplicationBuilder UseSwaggerUi3(
            this IApplicationBuilder app,
            Assembly webApiAssembly,
            Action<SwaggerUi3Settings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            return app.UseSwaggerUi3(new[] { webApiAssembly }, configure);
        }

        /// <summary>Adds the Swagger generator that uses reflection (legacy) and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssemblies">The Web API assemblies to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(NSwagServiceCollectionExtensions.AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerUi3) + "() instead.")]
        public static IApplicationBuilder UseSwaggerUi3(
            this IApplicationBuilder app,
            IEnumerable<Assembly> webApiAssemblies,
            Action<SwaggerUi3Settings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            var controllerTypes = webApiAssemblies.SelectMany(WebApiToSwaggerGenerator.GetControllerClasses);
            return app.UseSwaggerUi3(controllerTypes, configure);
        }

        /// <summary>Adds the Swagger generator that uses reflection (legacy) and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(NSwagServiceCollectionExtensions.AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerUi3) + "() instead.")]
        public static IApplicationBuilder UseSwaggerUi3(
            this IApplicationBuilder app,
            IEnumerable<Type> controllerTypes,
            Action<SwaggerUi3Settings<WebApiToSwaggerGeneratorSettings>> configure = null,
            SwaggerJsonSchemaGenerator schemaGenerator = null)
        {
            var settings = new SwaggerUi3Settings<WebApiToSwaggerGeneratorSettings>();
            configure?.Invoke(settings);

            if (controllerTypes != null)
                app.UseMiddleware<WebApiToSwaggerMiddleware>(settings.ActualSwaggerRoute, controllerTypes, settings, schemaGenerator ?? new SwaggerJsonSchemaGenerator(settings.GeneratorSettings));

            app.UseMiddleware<RedirectMiddleware>(settings.ActualSwaggerUiRoute, settings.ActualSwaggerRoute);
            app.UseMiddleware<SwaggerUiIndexMiddleware<WebApiToSwaggerGeneratorSettings>>(settings.ActualSwaggerUiRoute + "/index.html", settings, "NSwag.AspNetCore.SwaggerUi3.index.html");
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(settings.ActualSwaggerUiRoute),
                FileProvider = new EmbeddedFileProvider(typeof(SwaggerExtensions).GetTypeInfo().Assembly, "NSwag.AspNetCore.SwaggerUi3")
            });

            return app;
        }

        /// <summary>Adds the Swagger generator and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(NSwagServiceCollectionExtensions.AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerUi3) + "() instead.")]
        public static IApplicationBuilder UseSwaggerUi3WithApiExplorer(
            this IApplicationBuilder app,
            Action<SwaggerUi3Settings<AspNetCoreToSwaggerGeneratorSettings>> configure = null,
            SwaggerJsonSchemaGenerator schemaGenerator = null)
        {
            throw new NotSupportedException("Use " + nameof(NSwagServiceCollectionExtensions.AddSwagger) + "(), " + nameof(UseSwagger) +
                "() and " + nameof(UseSwaggerUi3) + "() instead.");
        }

        #endregion

        #region ReDoc

        /// <summary>Adds the Swagger generator that uses reflection (legacy) and ReDoc UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(NSwagServiceCollectionExtensions.AddSwagger) + "(), " + nameof(UseSwaggerReDoc) + "() and " + nameof(UseSwaggerUi3) + "() instead.")]
        public static IApplicationBuilder UseSwaggerReDoc(
            this IApplicationBuilder app,
            Assembly webApiAssembly,
            Action<SwaggerReDocSettings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            return app.UseSwaggerReDoc(new[] { webApiAssembly }, configure);
        }

        /// <summary>Adds the Swagger generator that uses reflection (legacy) and ReDoc UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssemblies">The Web API assemblies to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(NSwagServiceCollectionExtensions.AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerReDoc) + "() instead.")]
        public static IApplicationBuilder UseSwaggerReDoc(
            this IApplicationBuilder app,
            IEnumerable<Assembly> webApiAssemblies,
            Action<SwaggerReDocSettings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            var controllerTypes = webApiAssemblies.SelectMany(WebApiToSwaggerGenerator.GetControllerClasses);
            return app.UseSwaggerReDoc(controllerTypes, configure);
        }

        /// <summary>Adds the Swagger generator that uses reflection (legacy) and ReDoc UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(NSwagServiceCollectionExtensions.AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerReDoc) + "() instead.")]
        public static IApplicationBuilder UseSwaggerReDoc(
            this IApplicationBuilder app,
            IEnumerable<Type> controllerTypes,
            Action<SwaggerReDocSettings<WebApiToSwaggerGeneratorSettings>> configure = null,
            SwaggerJsonSchemaGenerator schemaGenerator = null)
        {
            var settings = new SwaggerReDocSettings<WebApiToSwaggerGeneratorSettings>();
            configure?.Invoke(settings);

            if (controllerTypes != null)
                app.UseMiddleware<WebApiToSwaggerMiddleware>(settings.ActualSwaggerRoute, controllerTypes, settings, schemaGenerator ?? new SwaggerJsonSchemaGenerator(settings.GeneratorSettings));

            app.UseMiddleware<RedirectMiddleware>(settings.ActualSwaggerUiRoute, settings.ActualSwaggerRoute);
            app.UseMiddleware<SwaggerUiIndexMiddleware<WebApiToSwaggerGeneratorSettings>>(settings.ActualSwaggerUiRoute + "/index.html", settings, "NSwag.AspNetCore.ReDoc.index.html");
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(settings.ActualSwaggerUiRoute),
                FileProvider = new EmbeddedFileProvider(typeof(SwaggerExtensions).GetTypeInfo().Assembly, "NSwag.AspNetCore.ReDoc")
            });

            return app;
        }

        /// <summary>Adds the Swagger generator and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(NSwagServiceCollectionExtensions.AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerReDoc) + "() instead.")]
        public static IApplicationBuilder UseSwaggerReDocWithApiExplorer(
            this IApplicationBuilder app,
            Action<SwaggerUiSettings<AspNetCoreToSwaggerGeneratorSettings>> configure = null,
            SwaggerJsonSchemaGenerator schemaGenerator = null)
        {
            throw new NotSupportedException("Use " + nameof(NSwagServiceCollectionExtensions.AddSwagger) + "(), " + nameof(UseSwagger) +
                "() and " + nameof(UseSwaggerReDoc) + "() instead.");
        }

        #endregion
    }
}
