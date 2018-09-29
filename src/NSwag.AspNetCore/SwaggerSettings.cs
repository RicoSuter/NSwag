//-----------------------------------------------------------------------
// <copyright file="SwaggerSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using NJsonSchema;
using NSwag.SwaggerGeneration;
using NSwag.SwaggerGeneration.WebApi;
using Newtonsoft.Json;

#if AspNetOwin
using Microsoft.Owin;

namespace NSwag.AspNet.Owin
#else
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace NSwag.AspNetCore
#endif
{
    /// <summary>The settings for UseSwagger.</summary>
    public class SwaggerSettings<T>
        where T : SwaggerGeneratorSettings, new()
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerSettings{T}"/> class.</summary>
        public SwaggerSettings()
        {
            GeneratorSettings = new T();

#if !AspNetOwin
            if (GeneratorSettings is WebApiToSwaggerGeneratorSettings)
                ((WebApiToSwaggerGeneratorSettings)(object)GeneratorSettings).IsAspNetCore = true;
#endif
        }

        /// <summary>Gets the generator settings.</summary>
        public T GeneratorSettings { get; }

        /// <summary>Gets or sets the OWIN base path (when mapped via app.MapOwinPath()) (must start with '/').</summary>
        public string MiddlewareBasePath { get; set; }

        /// <summary>Gets or sets the Swagger URL route (must start with '/').</summary>
        public string SwaggerRoute { get; set; } = "/swagger/v1/swagger.json";

        /// <summary>Gets or sets the Swagger post process action.</summary>
        public Action<SwaggerDocument> PostProcess { get; set; }

        /// <summary>Gets or sets for how long a <see cref="Exception"/> caught during schema generation is cached.</summary>
        public TimeSpan ExceptionCacheTime { get; set; } = TimeSpan.FromSeconds(10);

        internal virtual string ActualSwaggerRoute => SwaggerRoute.Substring(MiddlewareBasePath?.Length ?? 0);

        internal T CreateGeneratorSettings(JsonSerializerSettings serializerSettings, object mvcOptions)
        {
            GeneratorSettings.ApplySettings(serializerSettings, mvcOptions);
            return GeneratorSettings;
        }
    }
}