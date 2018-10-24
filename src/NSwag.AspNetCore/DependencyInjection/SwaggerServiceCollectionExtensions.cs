//-----------------------------------------------------------------------
// <copyright file="SwaggerServiceCollectionExtensions.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ApiDescription;
using Microsoft.Extensions.Options;
using NJsonSchema;
using NSwag.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> and <see cref="ISwaggerBuilder"/>.
    /// </summary>
    public static class SwaggerServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required for Swagger document generation to <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="configure">
        /// An <see cref="Action{T}"/> to configure the <see cref="SwaggerDocumentSettings"/>. If
        /// <see langword="null"/>, adds an Swagger document with the default configuration and name "v1".
        /// </param>
        /// <returns>An <see cref="ISwaggerBuilder"/> instance.</returns>
        /// <remarks>
        /// Reviewers: With this name and <c>using</c>s for both namespaces, the
        /// <see cref="SwaggerExtensions.AddSwagger(IServiceCollection)"/> method makes <c>AddSwagger()</c> ambiguous.
        /// Suggestions for a better name?
        /// </remarks>
        public static ISwaggerBuilder AddSwagger(
            this IServiceCollection services,
            Action<SwaggerDocumentSettings> configure = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services
                .AddSingleton<IConfigureOptions<MvcOptions>, NSwagConfigureMvcOptions>()
                .AddSingleton<DocumentRegistry>()
                .AddSingleton<SwaggerDocumentProvider>()

                // Used by the Microsoft.Extensions.ApiDescription tool
                .AddSingleton<IDocumentProvider>(s => s.GetRequiredService<SwaggerDocumentProvider>());

            var builder = new SwaggerBuilder(services);
            if (configure == null)
            {
                services.AddSingleton<SwaggerDocumentSettings>();
            }
            else
            {
                builder.AddSwaggerDocument(configure);
            }

            return builder;
        }

        /// <summary>
        /// Adds services required for Open API document generation to <paramref name="services"/>. Document added here
        /// will have <see cref="SwaggerDocumentSettings.GeneratorSettings"/> with <see cref="SchemaType.OpenApi3"/> by
        /// default.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="configure">
        /// An <see cref="Action{T}"/> to configure the <see cref="SwaggerDocumentSettings"/>. If
        /// <see langword="null"/>, adds an Open API document with the default configuration and name "v1".
        /// </param>
        /// <returns>An <see cref="ISwaggerBuilder"/> instance.</returns>
        public static ISwaggerBuilder AddOpenApi(
            this IServiceCollection services,
            Action<SwaggerDocumentSettings> configure = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services
                .AddSingleton<IConfigureOptions<MvcOptions>, NSwagConfigureMvcOptions>()
                .AddSingleton<DocumentRegistry>()
                .AddSingleton<SwaggerDocumentProvider>()

                // Used by the Microsoft.Extensions.ApiDescription tool
                .AddSingleton<IDocumentProvider>(s => s.GetRequiredService<SwaggerDocumentProvider>());

            var builder = new SwaggerBuilder(services);
            if (configure == null)
            {
                builder.AddOpenApiDocument(settings => { });
            }
            else
            {
                builder.AddOpenApiDocument(configure);
            }

            return builder;
        }
    }
}
