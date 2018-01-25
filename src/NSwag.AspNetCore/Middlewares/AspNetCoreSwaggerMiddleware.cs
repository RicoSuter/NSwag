//-----------------------------------------------------------------------
// <copyright file="SwaggerMiddleware.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using NSwag.SwaggerGeneration;
using NSwag.SwaggerGeneration.AspNetCore;

namespace NSwag.AspNetCore.Middlewares
{
    /// <summary>Generates a Swagger specification on a given path.</summary>
    public class AspNetCoreSwaggerMiddleware
    {
        private readonly RequestDelegate _nextDelegate;
        private readonly string _path;
        private readonly AspNetCoreToSwaggerMiddlewareSettings _settings;
        private readonly SwaggerJsonSchemaGenerator _schemaGenerator;
        private readonly IApiDescriptionGroupCollectionProvider _apiDescriptionGroupCollectionProvider;

        private int _version;
        private string _swaggerJson = null;

        /// <summary>Initializes a new instance of the <see cref="SwaggerMiddleware"/> class.</summary>
        /// <param name="nextDelegate">The next delegate.</param>
        /// <param name="apiDescriptionGroupCollectionProvider">The <see cref="IApiDescriptionGroupCollectionProvider"/>.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        public AspNetCoreSwaggerMiddleware(RequestDelegate nextDelegate, IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider, AspNetCoreToSwaggerMiddlewareSettings settings, SwaggerJsonSchemaGenerator schemaGenerator)
        {
            _nextDelegate = nextDelegate;
            _settings = settings;
            _path = settings.ActualSwaggerRoute;
            _schemaGenerator = schemaGenerator;
            _apiDescriptionGroupCollectionProvider = apiDescriptionGroupCollectionProvider;
        }

        /// <summary>Invokes the specified context.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The task.</returns>
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.HasValue && string.Equals(context.Request.Path.Value.Trim('/'), _path.Trim('/'), StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 200;
                context.Response.Headers["Content-Type"] = "application/json; charset=utf-8";
                await context.Response.WriteAsync(await GenerateSwaggerAsync(context));
            }
            else
                await _nextDelegate(context);
        }

        /// <summary>Generates the Swagger specification.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The Swagger specification.</returns>
        protected virtual async Task<string> GenerateSwaggerAsync(HttpContext context)
        {
            var apiDescriptionGroups = _apiDescriptionGroupCollectionProvider.ApiDescriptionGroups;
            if (apiDescriptionGroups.Version == Volatile.Read(ref _version) && _swaggerJson != null)
                return _swaggerJson;

            try
            {
                var generator = new AspNetCoreToSwaggerGenerator(_settings, _schemaGenerator);
                var document = await generator.GenerateAsync(apiDescriptionGroups);

                document.Host = context.Request.Host.Value ?? "";
                document.Schemes.Add(context.Request.Scheme == "http" ? SwaggerSchema.Http : SwaggerSchema.Https);
                document.BasePath = context.Request.PathBase.Value?.Substring(0, context.Request.PathBase.Value.Length - (_settings.MiddlewareBasePath?.Length ?? 0)) ?? "";

                _settings.PostProcess?.Invoke(document);
                _swaggerJson = document.ToJson();
                _version = apiDescriptionGroups.Version;
            }
            catch (Exception exception)
            {
                _swaggerJson = exception.ToString();
            }

            return _swaggerJson;
        }
    }
}
