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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using NSwag.AspNetCore.Middlewares;
using NSwag.SwaggerGeneration;
using NSwag.SwaggerGeneration.AspNetCore;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.AspNetCore
{
    /// <summary>Provides extensions to enable Swagger UI.</summary>
    public static class SwaggerExtensions
    {
        #region Service registration
      
        /// <summary>Adds services required for Swagger 2.0 generation (change document settings to generate OpenAPI 3.0).</summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configure">Configure the document registry.</param>
        public static IServiceCollection AddSwagger(this IServiceCollection serviceCollection, Action<SwaggerDocumentRegistry> configure = null)
        {
            if (configure == null)
            {
                configure = registry => registry.AddDocument();
            }

            serviceCollection.AddSingleton(s =>
            {
                var registry = new SwaggerDocumentRegistry();
                configure?.Invoke(registry);
                return registry;
            });

            serviceCollection.AddSingleton<IConfigureOptions<MvcOptions>, NSwagConfigureMvcOptions>();
            serviceCollection.AddSingleton<SwaggerDocumentProvider>();

            // Used by the Microsoft.Extensions.ApiDescription tool
            serviceCollection.AddSingleton<Microsoft.Extensions.ApiDescription.IDocumentProvider>(s =>
                s.GetRequiredService<SwaggerDocumentProvider>());

            return serviceCollection;
        }

        #endregion

        #region Swagger

        /// <summary>Adds the OpenAPI/Swagger generator that uses Api Description to perform Swagger generation.</summary>
        /// <remarks>This is the same as UseOpenApi(), configure the document in AddSwagger/AddOpenApi().</remarks>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure additional settings.</param>
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, Action<SwaggerMiddlewareSettings> configure = null)
        {
            return UseSwaggerWithApiExplorerCore(app, "v1", configure);
        }

        /// <summary>Adds the OpenAPI/Swagger generator that uses Api Description to perform Swagger generation.</summary>
        /// <remarks>This is the same as UseOpenApi(), configure the document in AddSwagger/AddOpenApi().</remarks>
        /// <param name="app">The app.</param>
        /// <param name="documentName">The document name (identifier from <see cref="AddSwagger(IServiceCollection, Action{SwaggerDocumentRegistry})"/>).</param>
        /// <param name="configure">Configure additional settings.</param>
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, string documentName, Action<SwaggerMiddlewareSettings> configure = null)
        {
            return UseSwaggerWithApiExplorerCore(app, documentName, configure);
        }

        #region Obsolete

        /// <summary>Adds the Swagger generator that uses Api Description to perform Swagger generation.</summary>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure additional settings.</param>
        [Obsolete("Use " + nameof(UseSwagger) + " instead.")]
        public static IApplicationBuilder UseSwaggerWithApiExplorer(this IApplicationBuilder app, Action<SwaggerMiddlewareSettings> configure = null)
        {
            return UseSwaggerWithApiExplorerCore(app, "v1", configure);
        }

        /// <summary>Adds the Swagger generator that uses Api Description to perform Swagger generation.</summary>
        /// <param name="app">The app.</param>
        /// <param name="documentName">The document name (identifier from <see cref="AddSwagger(IServiceCollection, Action{SwaggerDocumentRegistry})"/>).</param>
        /// <param name="configure">Configure additional settings.</param>
        [Obsolete("Use " + nameof(UseSwagger) + " instead.")]
        public static IApplicationBuilder UseSwaggerWithApiExplorer(this IApplicationBuilder app, string documentName, Action<SwaggerMiddlewareSettings> configure = null)
        {
            return UseSwaggerWithApiExplorerCore(app, documentName, configure);
        }

        /// <summary>Adds the Swagger generator that uses reflection (legacy) and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(AddSwagger) + "() and " + nameof(UseSwagger) + "() instead.")]
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
        [Obsolete("Use " + nameof(AddSwagger) + "() and " + nameof(UseSwagger) + "() instead.")]
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
        [Obsolete("Use " + nameof(AddSwagger) + "() and " + nameof(UseSwagger) + "() instead.")]
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

        #endregion

        #endregion

        #region SwaggerUi (Obsolete)

        /// <summary>Adds the Swagger generator and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerUi3) + "() instead.")]
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
        [Obsolete("Use " + nameof(AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerUi3) + "() instead.")]
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
        [Obsolete("Use " + nameof(AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerUi3) + "() instead.")]
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
        [Obsolete("Use " + nameof(AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerUi3) + "() instead.")]
        public static IApplicationBuilder UseSwaggerUiWithApiExplorer(
            this IApplicationBuilder app,
            Action<SwaggerUiSettings<AspNetCoreToSwaggerGeneratorSettings>> configure = null,
            SwaggerJsonSchemaGenerator schemaGenerator = null)
        {
            throw new NotSupportedException("Use " + nameof(AddSwagger) + "(), " + nameof(UseSwagger) +
                "() and " + nameof(UseSwaggerUi3) + "() instead.");
        }

        #endregion

        #region SwaggerUi3

        /// <summary>Adds the Swagger UI (only) to the pipeline.</summary>
        /// <remarks>The settings.GeneratorSettings property does not have any effect.</remarks>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerUi3(
            this IApplicationBuilder app,
            Action<SwaggerUi3Settings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            return app.UseSwaggerUi3(null, configure, null);
        }

        #region Obsolete

        /// <summary>Adds the Swagger generator that uses reflection (legacy) and Swagger UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerUi3) + "() instead.")]
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
        [Obsolete("Use " + nameof(AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerUi3) + "() instead.")]
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
        [Obsolete("Use " + nameof(AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerUi3) + "() instead.")]
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
        [Obsolete("Use " + nameof(AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerUi3) + "() instead.")]
        public static IApplicationBuilder UseSwaggerUi3WithApiExplorer(
            this IApplicationBuilder app,
            Action<SwaggerUi3Settings<AspNetCoreToSwaggerGeneratorSettings>> configure = null,
            SwaggerJsonSchemaGenerator schemaGenerator = null)
        {
            throw new NotSupportedException("Use " + nameof(AddSwagger) + "(), " + nameof(UseSwagger) + 
                "() and " + nameof(UseSwaggerUi3) + "() instead.");
        }

        #endregion

        #endregion

        #region ReDoc

        /// <summary>Adds the ReDoc UI (only) to the pipeline.</summary>
        /// <remarks>The settings.GeneratorSettings property does not have any effect.</remarks>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IApplicationBuilder UseSwaggerReDoc(
            this IApplicationBuilder app,
            Action<SwaggerReDocSettings<WebApiToSwaggerGeneratorSettings>> configure = null)
        {
            return app.UseSwaggerReDoc(null, configure, null);
        }

        #region Obsolete

        /// <summary>Adds the Swagger generator that uses reflection (legacy) and ReDoc UI to the pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(AddSwagger) + "(), " + nameof(UseSwaggerReDoc) + "() and " + nameof(UseSwaggerUi3) + "() instead.")]
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
        [Obsolete("Use " + nameof(AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerReDoc) + "() instead.")]
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
        [Obsolete("Use " + nameof(AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerReDoc) + "() instead.")]
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
        [Obsolete("Use " + nameof(AddSwagger) + "(), " + nameof(UseSwagger) + "() and " + nameof(UseSwaggerReDoc) + "() instead.")]
        public static IApplicationBuilder UseSwaggerReDocWithApiExplorer(
            this IApplicationBuilder app,
            Action<SwaggerUiSettings<AspNetCoreToSwaggerGeneratorSettings>> configure = null,
            SwaggerJsonSchemaGenerator schemaGenerator = null)
        {
            throw new NotSupportedException("Use " + nameof(AddSwagger) + "(), " + nameof(UseSwagger) +
                "() and " + nameof(UseSwaggerReDoc) + "() instead.");
        }

        #endregion

        #endregion

        private static IApplicationBuilder UseSwaggerWithApiExplorerCore(IApplicationBuilder app, string documentName, Action<SwaggerMiddlewareSettings> configure)
        {
            return app.UseMiddleware<SwaggerMiddleware>(documentName, configure ?? new Action<SwaggerMiddlewareSettings>((s) => { }));
        }
    }
}
