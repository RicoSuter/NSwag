//-----------------------------------------------------------------------
// <copyright file="SwaggerGeneratorSettingsFactoryBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NSwag.SwaggerGeneration
{
    /// <summary>The Swagger generator settings factory.</summary>
    /// <typeparam name="TSettings">The settings type.</typeparam>
    /// <typeparam name="TContext">The context type.</typeparam>
    public abstract class SwaggerGeneratorSettingsFactoryBase<TSettings, TContext> :
        ISwaggerGeneratorSettingsFactory<TSettings, TContext>
        where TSettings : SwaggerGeneratorSettings, new()
    {
        /// <summary>Cerates a new settings instance and with the correct serializer settings.</summary>
        /// <param name="serializerSettings">The serializer settings.</param>
        /// <param name="context">The context.</param>
        /// <returns>The task.</returns>
        public async Task<TSettings> CreateAsync(JsonSerializerSettings serializerSettings, TContext context)
        {
            var settings = new TSettings
            {
                SerializerSettings = serializerSettings
            };

            await ConfigureAsync(settings, context);
            return settings;
        }

        /// <summary>Configures a settings instance.</summary>
        /// <param name="settings">The settings.</param>
        /// <param name="context">The context.</param>
        /// <returns>The task.</returns>
        protected abstract Task ConfigureAsync(TSettings settings, TContext context);
    }
}