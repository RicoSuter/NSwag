//-----------------------------------------------------------------------
// <copyright file="NewtonsoftJsonOpenApiGeneratorSettingsExtensions.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.NewtonsoftJson.Generation;

namespace NSwag.Generation.NewtonsoftJson
{
    /// <summary>Extension methods to configure NSwag generators to use Newtonsoft.Json
    /// for schema generation (for applications that use AddNewtonsoftJson()).</summary>
    public static class NewtonsoftJsonOpenApiGeneratorSettingsExtensions
    {
        /// <summary>Configures the generator to use Newtonsoft.Json-based schema generation.
        /// Call this when your ASP.NET Core application uses AddNewtonsoftJson() on MVC.</summary>
        /// <param name="settings">The generator settings.</param>
        /// <param name="configure">Optional action to further configure the Newtonsoft schema settings.</param>
        /// <returns>The settings for chaining.</returns>
        public static OpenApiDocumentGeneratorSettings UseNewtonsoftJson(
            this OpenApiDocumentGeneratorSettings settings,
            Action<NewtonsoftJsonSchemaGeneratorSettings> configure = null)
        {
            var schemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
            {
                SchemaType = settings.SchemaSettings?.SchemaType ?? SchemaType.OpenApi3
            };

            configure?.Invoke(schemaSettings);
            settings.SchemaSettings = schemaSettings;
            return settings;
        }

        /// <summary>Configures the generator to use Newtonsoft.Json-based schema generation
        /// with the given serializer settings (typically from MvcNewtonsoftJsonOptions).</summary>
        /// <param name="settings">The generator settings.</param>
        /// <param name="serializerSettings">The Newtonsoft.Json serializer settings.</param>
        /// <returns>The settings for chaining.</returns>
        public static OpenApiDocumentGeneratorSettings UseNewtonsoftJson(
            this OpenApiDocumentGeneratorSettings settings,
            JsonSerializerSettings serializerSettings)
        {
            settings.SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
            {
                SchemaType = settings.SchemaSettings?.SchemaType ?? SchemaType.OpenApi3,
                SerializerSettings = serializerSettings
            };
            return settings;
        }
    }
}
