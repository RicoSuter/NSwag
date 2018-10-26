using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.AspNetCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>NSwag extensions for <see cref="IServiceCollection"/>.</summary>
    public static class NSwagServiceCollectionExtensions
    {
        /// <summary>Adds services required for Swagger 2.0 generation (change document settings to generate OpenAPI 3.0).</summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configure">Configure the document registry.</param>
        public static IServiceCollection AddSwagger(this IServiceCollection serviceCollection, Action<ISwaggerDocumentBuilder> configure = null)
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

            serviceCollection.AddSingleton<IConfigureOptions<MvcOptions>, SwaggerConfigureMvcOptions>();
            serviceCollection.AddSingleton<SwaggerDocumentProvider>();

            // Used by the Microsoft.Extensions.ApiDescription tool
            serviceCollection.AddSingleton<Microsoft.Extensions.ApiDescription.IDocumentProvider>(s =>
                s.GetRequiredService<SwaggerDocumentProvider>());

            return serviceCollection;
        }

        /// <summary>Adds a document to the registry.</summary>
        /// <param name="registry">The registry.</param>
        /// <param name="configure">The configure action.</param>
        /// <returns>The registry.</returns>
        public static ISwaggerDocumentBuilder AddDocument(this ISwaggerDocumentBuilder registry, Action<AspNetCoreToSwaggerGeneratorSettings> configure = null)
        {
            return AddDocument(registry, "v1", configure);
        }

        /// <summary>Adds a document to the registry.</summary>
        /// <param name="registry">The registry.</param>
        /// <param name="documentName">The document name.</param>
        /// <param name="configure">The configure action.</param>
        /// <returns>The registry.</returns>
        public static ISwaggerDocumentBuilder AddDocument(this ISwaggerDocumentBuilder registry, string documentName, Action<AspNetCoreToSwaggerGeneratorSettings> configure = null)
        {
            var settings = new AspNetCoreToSwaggerGeneratorSettings();
            configure?.Invoke(settings);

            var generator = new AspNetCoreToSwaggerGenerator(settings);
            return ((SwaggerDocumentRegistry)registry).AddDocument(documentName, generator);
        }
    }
}
