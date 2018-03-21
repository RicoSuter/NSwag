//-----------------------------------------------------------------------
// <copyright file="SwaggerMiddleware.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using NSwag.SwaggerGeneration;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.AspNet.Owin.Middlewares
{
    /// <summary>Generates a Swagger specification on a given path.</summary>
    public class SwaggerMiddleware : OwinMiddleware
    {
        private readonly string _path;
        private readonly SwaggerSettings<WebApiToSwaggerGeneratorSettings> _settings;
        private readonly IEnumerable<Type> _controllerTypes;
        private readonly SwaggerJsonSchemaGenerator _schemaGenerator;

        private string _schemaJson;
        private Exception _schemaException;
        private DateTimeOffset _schemaTimestamp;

        /// <summary>Initializes a new instance of the <see cref="SwaggerMiddleware"/> class.</summary>
        /// <param name="next">The next middleware.</param>
        /// <param name="path">The path.</param>
        /// <param name="controllerTypes">The controller types.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        public SwaggerMiddleware(OwinMiddleware next, string path, IEnumerable<Type> controllerTypes, SwaggerSettings<WebApiToSwaggerGeneratorSettings> settings, SwaggerJsonSchemaGenerator schemaGenerator)
            : base(next)
        {
            _path = path;
            _controllerTypes = controllerTypes;
            _settings = settings;
            _schemaGenerator = schemaGenerator;
        }

        /// <summary>Process an individual request.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The task.</returns>
        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Path.HasValue && string.Equals(context.Request.Path.Value.Trim('/'), _path.Trim('/'), StringComparison.OrdinalIgnoreCase))
            {
                var schemaJson = await GenerateSwaggerAsync(context);
                context.Response.StatusCode = 200;
                context.Response.Headers["Content-Type"] = "application/json; charset=utf-8";
                context.Response.Write(schemaJson);
            }
            else
                await Next.Invoke(context);
        }

        /// <summary>Generates the Swagger specification.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The Swagger specification.</returns>
        protected virtual async Task<string> GenerateSwaggerAsync(IOwinContext context)
        {
            if (_schemaException != null && _schemaTimestamp + _settings.ExceptionCacheTime > DateTimeOffset.UtcNow)
                throw _schemaException;

            if (_schemaJson == null)
            {
                try
                {
                    var generator = new WebApiToSwaggerGenerator(_settings.GeneratorSettings, _schemaGenerator);
                    var document = await generator.GenerateForControllersAsync(_controllerTypes);

                    document.Host = context.Request.Host.Value ?? "";
                    document.Schemes.Add(context.Request.Scheme == "http" ? SwaggerSchema.Http : SwaggerSchema.Https);
                    document.BasePath = context.Request.PathBase.Value?.Substring(0, context.Request.PathBase.Value.Length - (_settings.MiddlewareBasePath?.Length ?? 0)) ?? "";

                    _settings.PostProcess?.Invoke(document);
                    _schemaJson = document.ToJson();
                    _schemaException = null;
                    _schemaTimestamp = DateTimeOffset.UtcNow;
                }
                catch (Exception exception)
                {
                    _schemaJson = null;
                    _schemaException = exception;
                    _schemaTimestamp = DateTimeOffset.UtcNow;
                    throw _schemaException;
                }
            }

            return _schemaJson;
        }
    }
}