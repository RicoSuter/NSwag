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
using NSwag.AspNetCore.Middlewares;
using NSwag.SwaggerGeneration;
using NSwag.SwaggerGeneration.WebApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using NSwag.SwaggerGeneration.AspNetCore;

namespace NSwag.AspNetCore
{
    /// <summary>Provides extensions to enable Swagger UI.</summary>
    public static class SwaggerExtensions
    {
        #region Swagger

        /// <summary>Adds the Swagger generator and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwagger(
            this IApplicationBuilder app,
            Assembly webApiAssembly,
            Action<SwaggerSettings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            return app.UseSwagger(new[] { webApiAssembly }, configure);
        }

        /// <summary>Adds the Swagger generator and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssemblies">The Web API assemblies to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwagger(
            this IApplicationBuilder app,
            IEnumerable<Assembly> webApiAssemblies,
            Action<SwaggerSettings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            var controllerTypes = webApiAssemblies.SelectMany(WebApiToSwaggerGenerator.GetControllerClasses);
            return app.UseSwagger(controllerTypes, configure);
        }

        /// <summary>Adds the Swagger generator to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        /// <returns>The app builder.</returns>
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
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        [Obsolete("API Explorer support is experimental and the output may change until it's stable.")]
        public static IApplicationBuilder UseSwaggerWithApiExplorer(
            this IApplicationBuilder app,
            Action<SwaggerSettings<AspNetCoreToSwaggerGeneratorSettings>> configure = null,
            SwaggerJsonSchemaGenerator schemaGenerator = null)
        {
            var settings = new SwaggerSettings<AspNetCoreToSwaggerGeneratorSettings>();
            configure?.Invoke(settings);
            return app.UseMiddleware<AspNetCoreToSwaggerMiddleware>(settings, schemaGenerator ?? new SwaggerJsonSchemaGenerator(settings.GeneratorSettings));
        }

        /// <summary>Adds services required for swagger generation.</summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
        /// <remarks>
        /// This is currently only required in conjunction with 
        /// <see cref="SwaggerExtensions.UseSwaggerWithApiExplorer(IApplicationBuilder, Action{AspNetCoreToSwaggerMiddlewareSettings})"/>.
        /// </remarks>
        public static IServiceCollection AddSwagger(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton<IConfigureOptions<MvcOptions>, NSwagConfigureMvcOptions>();
        }

        #endregion

        #region SwaggerUi

        /// <summary>Adds the Swagger generator and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
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
        [Obsolete("API Explorer support is experimental and the output may change until it's stable.")]
        public static IApplicationBuilder UseSwaggerUiWithApiExplorer(
            this IApplicationBuilder app,
            Action<SwaggerUiSettings<AspNetCoreToSwaggerGeneratorSettings>> configure = null,
            SwaggerJsonSchemaGenerator schemaGenerator = null)
        {
            var settings = new SwaggerUiSettings<AspNetCoreToSwaggerGeneratorSettings>();
            configure?.Invoke(settings);

            app.UseMiddleware<AspNetCoreToSwaggerMiddleware>(settings, schemaGenerator ?? new SwaggerJsonSchemaGenerator(settings.GeneratorSettings));
            app.UseMiddleware<RedirectMiddleware>(settings.ActualSwaggerUiRoute, settings.ActualSwaggerRoute);
            app.UseMiddleware<SwaggerUiIndexMiddleware<AspNetCoreToSwaggerGeneratorSettings>>(settings.ActualSwaggerUiRoute + "/index.html", settings, "NSwag.AspNetCore.SwaggerUi.index.html");
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(settings.ActualSwaggerUiRoute),
                FileProvider = new EmbeddedFileProvider(typeof(SwaggerExtensions).GetTypeInfo().Assembly, "NSwag.AspNetCore.SwaggerUi")
            });

            return app;
        }

        #endregion

        #region SwaggerUi3

        /// <summary>Adds the Swagger generator and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerUi3(
            this IApplicationBuilder app,
            Assembly webApiAssembly,
            Action<SwaggerUi3Settings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            return app.UseSwaggerUi3(new[] { webApiAssembly }, configure);
        }

        /// <summary>Adds the Swagger generator and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssemblies">The Web API assemblies to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerUi3(
            this IApplicationBuilder app,
            IEnumerable<Assembly> webApiAssemblies,
            Action<SwaggerUi3Settings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            var controllerTypes = webApiAssemblies.SelectMany(WebApiToSwaggerGenerator.GetControllerClasses);
            return app.UseSwaggerUi3(controllerTypes, configure);
        }

        /// <summary>Adds the Swagger UI (only) to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerUi3(
            this IApplicationBuilder app,
            Action<SwaggerUi3Settings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            return app.UseSwaggerUi3(null, configure, null);
        }

        /// <summary>Adds the Swagger generator and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        /// <returns>The app builder.</returns>
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
        [Obsolete("API Explorer support is experimental and the output may change until it's stable.")]
        public static IApplicationBuilder UseSwaggerUi3WithApiExplorer(
            this IApplicationBuilder app,
            Action<SwaggerUiSettings<AspNetCoreToSwaggerGeneratorSettings>> configure = null,
            SwaggerJsonSchemaGenerator schemaGenerator = null)
        {
            var settings = new SwaggerUiSettings<AspNetCoreToSwaggerGeneratorSettings>();
            configure?.Invoke(settings);

            app.UseMiddleware<AspNetCoreToSwaggerMiddleware>(settings, schemaGenerator ?? new SwaggerJsonSchemaGenerator(settings.GeneratorSettings));
            app.UseMiddleware<RedirectMiddleware>(settings.ActualSwaggerUiRoute, settings.ActualSwaggerRoute);
            app.UseMiddleware<SwaggerUiIndexMiddleware<AspNetCoreToSwaggerGeneratorSettings>>(settings.ActualSwaggerUiRoute + "/index.html", settings, "NSwag.AspNetCore.SwaggerUi3.index.html");
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(settings.ActualSwaggerUiRoute),
                FileProvider = new EmbeddedFileProvider(typeof(SwaggerExtensions).GetTypeInfo().Assembly, "NSwag.AspNetCore.SwaggerUi3")
            });

            return app;
        }

        #endregion

        #region ReDoc

        /// <summary>Adds the Swagger generator and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerReDoc(
            this IApplicationBuilder app,
            Assembly webApiAssembly,
            Action<SwaggerReDocSettings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            return app.UseSwaggerReDoc(new[] { webApiAssembly }, configure);
        }

        /// <summary>Adds the Swagger generator and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssemblies">The Web API assemblies to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerReDoc(
            this IApplicationBuilder app,
            IEnumerable<Assembly> webApiAssemblies,
            Action<SwaggerReDocSettings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            var controllerTypes = webApiAssemblies.SelectMany(WebApiToSwaggerGenerator.GetControllerClasses);
            return app.UseSwaggerReDoc(controllerTypes, configure);
        }

        /// <summary>Adds the Swagger UI (only) to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerReDoc(
            this IApplicationBuilder app,
            Action<SwaggerReDocSettings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            return app.UseSwaggerReDoc(null, configure, null);
        }

        /// <summary>Adds the Swagger generator and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        /// <returns>The app builder.</returns>
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
        [Obsolete("API Explorer support is experimental and the output may change until it's stable.")]
        public static IApplicationBuilder UseSwaggerReDocWithApiExplorer(
            this IApplicationBuilder app,
            Action<SwaggerUiSettings<AspNetCoreToSwaggerGeneratorSettings>> configure = null,
            SwaggerJsonSchemaGenerator schemaGenerator = null)
        {
            var settings = new SwaggerUiSettings<AspNetCoreToSwaggerGeneratorSettings>();
            configure?.Invoke(settings);

            app.UseMiddleware<AspNetCoreToSwaggerMiddleware>(settings, schemaGenerator ?? new SwaggerJsonSchemaGenerator(settings.GeneratorSettings));
            app.UseMiddleware<RedirectMiddleware>(settings.ActualSwaggerUiRoute, settings.ActualSwaggerRoute);
            app.UseMiddleware<SwaggerUiIndexMiddleware<AspNetCoreToSwaggerGeneratorSettings>>(settings.ActualSwaggerUiRoute + "/index.html", settings, "NSwag.AspNetCore.ReDoc.index.html");
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(settings.ActualSwaggerUiRoute),
                FileProvider = new EmbeddedFileProvider(typeof(SwaggerExtensions).GetTypeInfo().Assembly, "NSwag.AspNetCore.ReDoc")
            });

            return app;
        }

        #endregion
    }
}
