//-----------------------------------------------------------------------
// <copyright file="ISwaggerGeneratorSettingsFactory.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NSwag.SwaggerGeneration
{
    /// <summary>The Swagger generator settings factory interface.</summary>
    /// <typeparam name="TSettings">The settings type.</typeparam>
    /// <typeparam name="TContext">The context type.</typeparam>
    public interface ISwaggerGeneratorSettingsFactory<TSettings, in TContext>
    {
        /// <summary>Cerates a new settings instance and with the correct serializer settings.</summary>
        /// <param name="serializerSettings">The serializer settings.</param>
        /// <param name="context">The context.</param>
        /// <returns>The task.</returns>
        Task<TSettings> CreateAsync(JsonSerializerSettings serializerSettings, TContext context);
    }
}
