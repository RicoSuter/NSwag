//-----------------------------------------------------------------------
// <copyright file="SwaggerExtensions.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using NSwag.AspNet.Owin.Middlewares;
using NSwag.SwaggerGeneration;
using NSwag.SwaggerGeneration.WebApi;
using Owin;

namespace NSwag.AspNet.Owin
{
    using Microsoft.Web.Http.Description;
    using ODataToSwaggerTest.Middlewares;
    using SwaggerGeneration.WebApi.Versioned;

    /// <summary>Provides OWIN extensions to enable Swagger UI.</summary>
    public static class SwaggerExtensions
    {
        public static IAppBuilder UseVersionedSwagger(
            this IAppBuilder app,
            VersionedApiExplorer explorer,
            Action<SwaggerSettings<VersionedWebApiToSwaggerGeneratorSettings>> configure = null,
            SwaggerJsonSchemaGenerator schemaGenerator = null)
        {
            var settings = new SwaggerSettings<VersionedWebApiToSwaggerGeneratorSettings>();
            configure?.Invoke(settings);
            
            return app.Use(typeof(VersionedSwaggerDocumentMiddleware), explorer, settings, schemaGenerator ?? new SwaggerJsonSchemaGenerator(settings.GeneratorSettings));
        }        
    }
}
