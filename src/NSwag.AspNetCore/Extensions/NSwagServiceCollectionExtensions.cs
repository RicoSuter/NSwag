using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NJsonSchema;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration;
using NSwag.SwaggerGeneration.AspNetCore;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>NSwag extensions for <see cref="IServiceCollection"/>.</summary>
    public static class NSwagServiceCollectionExtensions
    {
        /// <summary>Adds services required for Swagger 2.0 generation (change document settings to generate OpenAPI 3.0).</summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configure">Configure the document.</param>
        public static IServiceCollection AddOpenApiDocument(this IServiceCollection serviceCollection, Action<SwaggerDocumentSettings> configure = null)
        {
            return AddSwaggerDocument(serviceCollection, settings =>
            {
                settings.SchemaType = SchemaType.OpenApi3;
                configure?.Invoke(settings);
            });
        }

        /// <summary>Adds services required for Swagger 2.0 generation (change document settings to generate OpenAPI 3.0).</summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configure">Configure the document.</param>
        public static IServiceCollection AddSwaggerDocument(this IServiceCollection serviceCollection, Action<SwaggerDocumentSettings> configure = null)
        {
            serviceCollection.AddSingleton(s =>
            {
                var settings = new SwaggerDocumentSettings();
                settings.SchemaType = SchemaType.Swagger2;

                configure?.Invoke(settings);

                var generator = new AspNetCoreToSwaggerGenerator(settings, settings.SchemaGenerator ??
                    new SwaggerJsonSchemaGenerator(settings));

                return new SwaggerDocumentRegistration(settings.DocumentName, generator);
            });

            var descriptor = serviceCollection.SingleOrDefault(d => d.ServiceType == typeof(SwaggerDocumentProvider));
            if (descriptor == null)
            {
                serviceCollection.AddSingleton<SwaggerDocumentProvider>();
                serviceCollection.AddSingleton<IConfigureOptions<MvcOptions>, SwaggerConfigureMvcOptions>();

                // Used by UseDocumentProvider CLI setting
                serviceCollection.AddSingleton<ISwaggerDocumentProvider>(s => s.GetRequiredService<SwaggerDocumentProvider>());

                // Used by the Microsoft.Extensions.ApiDescription tool
                serviceCollection.AddSingleton<ApiDescription.IDocumentProvider>(s => s.GetRequiredService<SwaggerDocumentProvider>());
            }

            return serviceCollection;
        }

        /// <summary>Adds services required for Swagger 2.0 generation (change document settings to generate OpenAPI 3.0).</summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configure">Configure the document.</param>
        [Obsolete("Use " + nameof(AddSwaggerDocument) + "() instead.")]
        public static IServiceCollection AddSwagger(this IServiceCollection serviceCollection, Action<SwaggerDocumentSettings> configure = null)
        {
            return AddSwaggerDocument(serviceCollection, configure);
        }
    }
}
