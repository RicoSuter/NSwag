//-----------------------------------------------------------------------
// <copyright file="DocumentRegistry.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSwag.SwaggerGeneration;
using NSwag.SwaggerGeneration.AspNetCore;
using System;
using System.Threading.Tasks;

namespace NSwag.AspNetCore.Documents
{
    /// <summary>The document based on the <see cref="AspNetCoreToSwaggerGeneratorSettings"/> settings.</summary>
    public class AspNetCoreToSwaggerDocument : AspNetCoreToSwaggerGeneratorSettings, ISwaggerDocument
    {
        /// <summary>Gets or sets the used <see cref="SwaggerJsonSchemaGenerator"/>.</summary>
        public SwaggerJsonSchemaGenerator SchemaGenerator { get; set; }

        /// <summary>Generates the <see cref="SwaggerDocument"/>.</summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>The document</returns>
        public async Task<SwaggerDocument> GenerateAsync(IServiceProvider serviceProvider)
        {
            var mvcOptions = serviceProvider.GetRequiredService<IOptions<MvcOptions>>();
            var mvcJsonOptions = serviceProvider.GetRequiredService<IOptions<MvcJsonOptions>>();
            var apiDescriptionGroupCollectionProvider = serviceProvider.GetRequiredService<IApiDescriptionGroupCollectionProvider>();

            ApplySettings(mvcJsonOptions.Value.SerializerSettings, mvcOptions.Value);

            var generator = new AspNetCoreToSwaggerGenerator(this, SchemaGenerator ?? new SwaggerJsonSchemaGenerator(this));
            return await generator.GenerateAsync(apiDescriptionGroupCollectionProvider.ApiDescriptionGroups);
        }
    }
}
