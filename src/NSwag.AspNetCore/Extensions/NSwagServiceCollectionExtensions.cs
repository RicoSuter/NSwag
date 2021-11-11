//-----------------------------------------------------------------------
// <copyright file="NSwagServiceCollectionExtensions.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ApiDescriptions;
using Microsoft.Extensions.Options;
using NJsonSchema;
using NSwag.AspNetCore;
using NSwag.Generation;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>NSwag extensions for <see cref="IServiceCollection"/>.</summary>
    public static class NSwagServiceCollectionExtensions
    {
        /// <summary>Adds services required for OpenAPI 3.0 generation (change document settings to generate Swagger 2.0).</summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configure">Configure the document.</param>
        public static IServiceCollection AddOpenApiDocument(this IServiceCollection serviceCollection, Action<AspNetCoreOpenApiDocumentGeneratorSettings> configure)
        {
            return AddOpenApiDocument(serviceCollection, (settings, services) =>
            {
                configure?.Invoke(settings);
            });
        }

        /// <summary>Adds services required for OpenAPI 3.0 generation (change document settings to generate Swagger 2.0).</summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configure">Configure the document.</param>
        public static IServiceCollection AddOpenApiDocument(this IServiceCollection serviceCollection, Action<AspNetCoreOpenApiDocumentGeneratorSettings, IServiceProvider> configure = null)
        {
            return AddSwaggerDocument(serviceCollection, (settings, services) =>
            {
                settings.SchemaType = SchemaType.OpenApi3;
                configure?.Invoke(settings, services);
            });
        }

        /// <summary>Adds services required for Swagger 2.0 generation (change document settings to generate OpenAPI 3.0).</summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configure">Configure the document.</param>
        public static IServiceCollection AddSwaggerDocument(this IServiceCollection serviceCollection, Action<AspNetCoreOpenApiDocumentGeneratorSettings> configure)
        {
            return AddSwaggerDocument(serviceCollection, (settings, services) =>
            {
                configure?.Invoke(settings);
            });
        }

        /// <summary>Adds services required for Swagger 2.0 generation (change document settings to generate OpenAPI 3.0).</summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configure">Configure the document.</param>
        public static IServiceCollection AddSwaggerDocument(this IServiceCollection serviceCollection, Action<AspNetCoreOpenApiDocumentGeneratorSettings, IServiceProvider> configure = null)
        {
            serviceCollection.AddSingleton(services =>
            {
                var settings = new AspNetCoreOpenApiDocumentGeneratorSettings
                {
                    SchemaType = SchemaType.Swagger2,
                };

                configure?.Invoke(settings, services);

                foreach (var documentProcessor in services.GetRequiredService<IEnumerable<IDocumentProcessor>>())
                {
                    settings.DocumentProcessors.Add(documentProcessor);
                }

                foreach (var operationProcessor in services.GetRequiredService<IEnumerable<IOperationProcessor>>())
                {
                    settings.OperationProcessors.Add(operationProcessor);
                }

                var generator = new AspNetCoreOpenApiDocumentGenerator(settings);
                return new OpenApiDocumentRegistration(settings.DocumentName, generator);
            });

            var descriptor = serviceCollection.SingleOrDefault(d => d.ServiceType == typeof(OpenApiDocumentProvider));
            if (descriptor == null)
            {
                serviceCollection.AddSingleton<OpenApiDocumentProvider>();
                serviceCollection.AddSingleton<IConfigureOptions<MvcOptions>, OpenApiConfigureMvcOptions>();

                // Used by UseDocumentProvider CLI setting
                serviceCollection.AddSingleton<IOpenApiDocumentGenerator>(s => s.GetRequiredService<OpenApiDocumentProvider>());

                // Used by the <c>dotnet-getdocument</c> tool from the Microsoft.Extensions.ApiDescription.Server package.
                serviceCollection.AddSingleton<IDocumentProvider>(s => s.GetRequiredService<OpenApiDocumentProvider>());
            }

            return serviceCollection;
        }

        /// <summary>Adds services required for Swagger 2.0 generation (change document settings to generate OpenAPI 3.0).</summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configure">Configure the document.</param>
        [Obsolete("Use " + nameof(AddSwaggerDocument) + "() instead.")]
        public static IServiceCollection AddSwagger(this IServiceCollection serviceCollection, Action<AspNetCoreOpenApiDocumentGeneratorSettings> configure = null)
        {
            return AddSwaggerDocument(serviceCollection, configure);
        }
    }
}
