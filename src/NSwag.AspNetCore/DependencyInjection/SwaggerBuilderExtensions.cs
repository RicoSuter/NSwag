//-----------------------------------------------------------------------
// <copyright file="SwaggerBuilderExtensions.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using NJsonSchema;
using NSwag.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for <see cref="ISwaggerBuilder"/>.
    /// </summary>
    public static class SwaggerBuilderExtensions
    {
        /// <summary>
        /// Add an Open API document to <paramref name="builder"/>. Document added here will have
        /// <see cref="SwaggerDocumentSettings.GeneratorSettings"/> with <see cref="SchemaType.OpenApi3"/> by default.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        /// <remarks>
        /// Reviewers: Could allow <paramref name="configure"/> to be <see langword="null"/>. But, it seems odd for
        /// later document additions to use the default configuration. Reserved that for
        /// <see cref="SwaggerServiceCollectionExtensions.AddOpenApi"/>.
        /// </remarks>
        public static ISwaggerBuilder AddOpenApiDocument(
            this ISwaggerBuilder builder,
            Action<SwaggerDocumentSettings> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var settings = new SwaggerDocumentSettings();
            settings.GeneratorSettings.SchemaType = SchemaType.OpenApi3;
            configure(settings);
            builder.Services.AddSingleton(settings);

            return builder;
        }

        /// <summary>
        /// Add an Swagger document to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        /// <remarks>
        /// Reviewers: Could allow <paramref name="configure"/> to be <see langword="null"/>. But, it seems odd for
        /// later document additions to use the default configuration. Reserved that for
        /// <see cref="SwaggerServiceCollectionExtensions.AddSwagger"/>.
        /// </remarks>
        public static ISwaggerBuilder AddSwaggerDocument(
            this ISwaggerBuilder builder,
            Action<SwaggerDocumentSettings> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var settings = new SwaggerDocumentSettings();
            configure(settings);
            builder.Services.AddSingleton(settings);

            return builder;
        }
    }
}
