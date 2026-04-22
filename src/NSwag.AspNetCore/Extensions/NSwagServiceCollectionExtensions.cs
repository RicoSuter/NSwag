//-----------------------------------------------------------------------
// <copyright file="NSwagServiceCollectionExtensions.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ApiDescriptions;
using Microsoft.Extensions.Options;
using NJsonSchema;
using NJsonSchema.Generation;
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
                settings.SchemaSettings.SchemaType = SchemaType.OpenApi3;
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
                var settings = new AspNetCoreOpenApiDocumentGeneratorSettings();

                var mvcOptions = services.GetRequiredService<IOptions<MvcOptions>>();
                var hasSystemTextJsonOutputFormatter = mvcOptions.Value.OutputFormatters
                    .Any(f => f.GetType().Name == "SystemTextJsonOutputFormatter");

#pragma warning disable CS0618 // Obsolete: we use the reflection-based resolver directly here
                var newtonsoftSettings = AspNetCoreOpenApiDocumentGenerator.GetJsonSerializerSettings(services);
#pragma warning restore CS0618
                var systemTextJsonOptions = hasSystemTextJsonOutputFormatter
                    ? AspNetCoreOpenApiDocumentGenerator.GetSystemTextJsonSettings(services)
#if NET6_0_OR_GREATER
                    : services.GetService<IOptions<AspNetCore.Http.Json.JsonOptions>>()?.Value.SerializerOptions;
#else 
                    : null;
#endif

                if (newtonsoftSettings != null && !hasSystemTextJsonOutputFormatter)
                {
                    var newtonsoftSchemaSettings = TryCreateNewtonsoftJsonSchemaGeneratorSettings(newtonsoftSettings);
                    if (newtonsoftSchemaSettings != null)
                    {
                        settings.ApplySettings(newtonsoftSchemaSettings, mvcOptions.Value);
                    }
                    else
                    {
                        settings.ApplySettings(new SystemTextJsonSchemaGeneratorSettings(), mvcOptions.Value);
                    }
                }
                else if (systemTextJsonOptions != null)
                {
                    settings.ApplySettings(new SystemTextJsonSchemaGeneratorSettings { SerializerOptions = systemTextJsonOptions }, mvcOptions.Value);
                }
                else
                {
                    settings.ApplySettings(new SystemTextJsonSchemaGeneratorSettings(), mvcOptions.Value);
                }

                settings.SchemaSettings.SchemaType = SchemaType.Swagger2;

                configure?.Invoke(settings, services);

                foreach (var documentProcessor in services.GetRequiredService<IEnumerable<IDocumentProcessor>>())
                {
                    settings.DocumentProcessors.Add(documentProcessor);
                }

                foreach (var operationProcessor in services.GetRequiredService<IEnumerable<IOperationProcessor>>())
                {
                    settings.OperationProcessors.Add(operationProcessor);
                }

                return new OpenApiDocumentRegistration(settings.DocumentName, settings);
            });

            var descriptor = serviceCollection.SingleOrDefault(d => d.ServiceType == typeof(OpenApiDocumentProvider));
            if (descriptor == null)
            {
                serviceCollection.AddSingleton<OpenApiDocumentProvider>();
                serviceCollection.AddSingleton<IConfigureOptions<MvcOptions>, OpenApiConfigureMvcOptions>();
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

        private static JsonSchemaGeneratorSettings TryCreateNewtonsoftJsonSchemaGeneratorSettings(object newtonsoftSettings)
        {
            try
            {
                var assembly = Assembly.Load(new AssemblyName("NJsonSchema.NewtonsoftJson"));
                var settingsType = assembly.GetType("NJsonSchema.NewtonsoftJson.Generation.NewtonsoftJsonSchemaGeneratorSettings");
                if (settingsType == null)
                {
                    return null;
                }

                var instance = Activator.CreateInstance(settingsType);
                settingsType.GetProperty("SerializerSettings")?.SetValue(instance, newtonsoftSettings);
                return instance as JsonSchemaGeneratorSettings;
            }
            catch
            {
                return null;
            }
        }
    }
}
