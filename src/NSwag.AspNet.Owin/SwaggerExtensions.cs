//-----------------------------------------------------------------------
// <copyright file="SwaggerExtensions.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
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
using NSwag.AspNet.Owin.Middlewares;
using NSwag.Generation;
using NSwag.Generation.WebApi;
using Owin;

namespace NSwag.AspNet.Owin
{
    /// <summary>Provides OWIN extensions to enable Swagger UI.</summary>
    public static class SwaggerExtensions
    {
        #region Swagger

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IAppBuilder UseSwagger(
            this IAppBuilder app,
            Assembly webApiAssembly,
            Action<SwaggerSettings<WebApiOpenApiDocumentGeneratorSettings>> configure = null)
        {
            return app.UseSwagger(new[] { webApiAssembly }, configure);
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssemblies">The Web API assemblies to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IAppBuilder UseSwagger(
            this IAppBuilder app,
            IEnumerable<Assembly> webApiAssemblies,
            Action<SwaggerSettings<WebApiOpenApiDocumentGeneratorSettings>> configure = null)
        {
            var controllerTypes = webApiAssemblies.SelectMany(WebApiOpenApiDocumentGenerator.GetControllerClasses);
            return app.UseSwagger(controllerTypes, configure);
        }

        /// <summary>Addes the Swagger generator to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IAppBuilder UseSwagger(
            this IAppBuilder app,
            IEnumerable<Type> controllerTypes,
            Action<SwaggerSettings<WebApiOpenApiDocumentGeneratorSettings>> configure = null)
        {
            var settings = new SwaggerSettings<WebApiOpenApiDocumentGeneratorSettings>();
            configure?.Invoke(settings);

            app.Use<OpenApiDocumentMiddleware>(settings.ActualSwaggerDocumentPath, controllerTypes, settings);
            app.UseStageMarker(PipelineStage.MapHandler);
            return app;
        }

        #endregion

        #region SwaggerUi

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(UseSwaggerUi3) + " instead.")]
        public static IAppBuilder UseSwaggerUi(
            this IAppBuilder app,
            Assembly webApiAssembly,
            Action<SwaggerUiSettings<WebApiOpenApiDocumentGeneratorSettings>> configure = null)
        {
            return app.UseSwaggerUi(new[] { webApiAssembly }, configure);
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssemblies">The Web API assemblies to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(UseSwaggerUi3) + " instead.")]
        public static IAppBuilder UseSwaggerUi(
            this IAppBuilder app,
            IEnumerable<Assembly> webApiAssemblies,
            Action<SwaggerUiSettings<WebApiOpenApiDocumentGeneratorSettings>> configure = null)
        {
            var controllerTypes = webApiAssemblies.SelectMany(WebApiOpenApiDocumentGenerator.GetControllerClasses);
            return app.UseSwaggerUi(controllerTypes, configure);
        }

        /// <summary>Addes the Swagger UI (only) to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(UseSwaggerUi3) + " instead.")]
        public static IAppBuilder UseSwaggerUi(
            this IAppBuilder app,
            Action<SwaggerSettings<WebApiOpenApiDocumentGeneratorSettings>> configure = null)
        {
            return app.UseSwaggerUi((IEnumerable<Type>)null, configure);
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        [Obsolete("Use " + nameof(UseSwaggerUi3) + " instead.")]
        public static IAppBuilder UseSwaggerUi(
            this IAppBuilder app,
            IEnumerable<Type> controllerTypes,
            Action<SwaggerUiSettings<WebApiOpenApiDocumentGeneratorSettings>> configure = null)
        {
            var settings = new SwaggerUiSettings<WebApiOpenApiDocumentGeneratorSettings>();
            configure?.Invoke(settings);

            if (controllerTypes != null)
            {
                app.Use<OpenApiDocumentMiddleware>(settings.ActualSwaggerDocumentPath, controllerTypes, settings);
            }

            app.Use<RedirectToIndexMiddleware>(settings.ActualSwaggerUiPath, settings.ActualSwaggerDocumentPath, settings.TransformToExternalPath);
            app.Use<SwaggerUiIndexMiddleware<WebApiOpenApiDocumentGeneratorSettings>>(settings.ActualSwaggerUiPath + "/index.html", settings, "NSwag.AspNet.Owin.SwaggerUi.index.html");
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(settings.ActualSwaggerUiPath),
                FileSystem = new EmbeddedResourceFileSystem(typeof(SwaggerExtensions).Assembly, "NSwag.AspNet.Owin.SwaggerUi")
            });
            app.UseStageMarker(PipelineStage.MapHandler);
            return app;
        }

        #endregion

        #region SwaggerUi3

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IAppBuilder UseSwaggerUi3(
            this IAppBuilder app,
            Assembly webApiAssembly,
            Action<SwaggerUi3Settings<WebApiOpenApiDocumentGeneratorSettings>> configure = null)
        {
            return app.UseSwaggerUi3(new[] { webApiAssembly }, configure);
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssemblies">The Web API assemblies to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IAppBuilder UseSwaggerUi3(
            this IAppBuilder app,
            IEnumerable<Assembly> webApiAssemblies,
            Action<SwaggerUi3Settings<WebApiOpenApiDocumentGeneratorSettings>> configure = null)
        {
            var controllerTypes = webApiAssemblies.SelectMany(WebApiOpenApiDocumentGenerator.GetControllerClasses);
            return app.UseSwaggerUi3(controllerTypes, configure);
        }

        /// <summary>Addes the Swagger UI (only) to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IAppBuilder UseSwaggerUi3(
            this IAppBuilder app,
            Action<SwaggerUi3Settings<WebApiOpenApiDocumentGeneratorSettings>> configure = null)
        {
            return app.UseSwaggerUi3((IEnumerable<Type>)null, configure);
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IAppBuilder UseSwaggerUi3(
            this IAppBuilder app,
            IEnumerable<Type> controllerTypes,
            Action<SwaggerUi3Settings<WebApiOpenApiDocumentGeneratorSettings>> configure = null)
        {
            var settings = new SwaggerUi3Settings<WebApiOpenApiDocumentGeneratorSettings>();
            configure?.Invoke(settings);

            if (controllerTypes != null)
            {
                app.Use<OpenApiDocumentMiddleware>(settings.ActualSwaggerDocumentPath, controllerTypes, settings);
            }

            app.Use<RedirectToIndexMiddleware>(settings.ActualSwaggerUiPath, settings.ActualSwaggerDocumentPath, settings.TransformToExternalPath);
            app.Use<SwaggerUiIndexMiddleware<WebApiOpenApiDocumentGeneratorSettings>>(settings.ActualSwaggerUiPath + "/index.html", settings, "NSwag.AspNet.Owin.SwaggerUi3.index.html");
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(settings.ActualSwaggerUiPath),
                FileSystem = new EmbeddedResourceFileSystem(typeof(SwaggerExtensions).Assembly, "NSwag.AspNet.Owin.SwaggerUi3")
            });
            app.UseStageMarker(PipelineStage.MapHandler);
            return app;
        }

        #endregion

        #region ReDoc

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssembly">The Web API assembly to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IAppBuilder UseSwaggerReDoc(
            this IAppBuilder app,
            Assembly webApiAssembly,
            Action<ReDocSettings<WebApiOpenApiDocumentGeneratorSettings>> configure = null)
        {
            return app.UseSwaggerReDoc(new[] { webApiAssembly }, configure);
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="webApiAssemblies">The Web API assemblies to search for controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IAppBuilder UseSwaggerReDoc(
            this IAppBuilder app,
            IEnumerable<Assembly> webApiAssemblies,
            Action<ReDocSettings<WebApiOpenApiDocumentGeneratorSettings>> configure = null)
        {
            var controllerTypes = webApiAssemblies.SelectMany(WebApiOpenApiDocumentGenerator.GetControllerClasses);
            return app.UseSwaggerReDoc(controllerTypes, configure);
        }

        /// <summary>Addes the Swagger UI (only) to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IAppBuilder UseSwaggerReDoc(
            this IAppBuilder app,
            Action<ReDocSettings<WebApiOpenApiDocumentGeneratorSettings>> configure = null)
        {
            return app.UseSwaggerReDoc((IEnumerable<Type>)null, configure);
        }

        /// <summary>Addes the Swagger generator and Swagger UI to the OWIN pipeline.</summary>
        /// <param name="app">The app.</param>
        /// <param name="controllerTypes">The Web API controller types.</param>
        /// <param name="configure">Configure the Swagger settings.</param>
        /// <returns>The app builder.</returns>
        public static IAppBuilder UseSwaggerReDoc(
            this IAppBuilder app,
            IEnumerable<Type> controllerTypes,
            Action<ReDocSettings<WebApiOpenApiDocumentGeneratorSettings>> configure = null)
        {
            var settings = new ReDocSettings<WebApiOpenApiDocumentGeneratorSettings>();
            configure?.Invoke(settings);

            if (controllerTypes != null)
            {
                app.Use<OpenApiDocumentMiddleware>(settings.ActualSwaggerDocumentPath, controllerTypes, settings);
            }

            app.Use<RedirectToIndexMiddleware>(settings.ActualSwaggerUiPath, settings.ActualSwaggerDocumentPath, settings.TransformToExternalPath);
            app.Use<SwaggerUiIndexMiddleware<WebApiOpenApiDocumentGeneratorSettings>>(settings.ActualSwaggerUiPath + "/index.html", settings, "NSwag.AspNet.Owin.ReDoc.index.html");
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(settings.ActualSwaggerUiPath),
                FileSystem = new EmbeddedResourceFileSystem(typeof(SwaggerExtensions).Assembly, "NSwag.AspNet.Owin.ReDoc")
            });
            app.UseStageMarker(PipelineStage.MapHandler);
            return app;
        }

        #endregion
    }
}
