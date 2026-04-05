//-----------------------------------------------------------------------
// <copyright file="NewtonsoftJsonSettingsResolver.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Reflection;
using Newtonsoft.Json;

namespace NSwag.Generation.NewtonsoftJson
{
    /// <summary>Resolves Newtonsoft.Json serializer settings from ASP.NET Core's DI container.</summary>
    public static class NewtonsoftJsonSettingsResolver
    {
        /// <summary>Loads the Newtonsoft.Json <see cref="JsonSerializerSettings"/> from the given
        /// service provider by detecting MvcNewtonsoftJsonOptions via reflection.</summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>The settings, or null if Newtonsoft.Json is not configured.</returns>
        public static JsonSerializerSettings GetJsonSerializerSettings(IServiceProvider serviceProvider)
        {
            try
            {
                var optionsAssembly = Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc.NewtonsoftJson"));
                var iOptionsType = Type.GetType("Microsoft.Extensions.Options.IOptions`1, Microsoft.Extensions.Options");
                if (iOptionsType == null)
                {
                    return null;
                }

                var optionsType = iOptionsType.MakeGenericType(
                    optionsAssembly.GetType("Microsoft.AspNetCore.Mvc.MvcNewtonsoftJsonOptions", true));
                var options = serviceProvider?.GetService(optionsType);

                return (JsonSerializerSettings)((dynamic)options?.GetType().GetProperty("Value")?.GetValue(options))?.SerializerSettings;
            }
            catch
            {
                return null;
            }
        }
    }
}
