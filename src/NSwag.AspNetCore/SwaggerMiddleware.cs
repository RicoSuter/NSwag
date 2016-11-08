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
using Microsoft.AspNetCore.Http;
using NSwag.CodeGeneration.SwaggerGenerators;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.AspNetCore
{
    /// <summary>Generates a Swagger specification on a given path.</summary>
    public class SwaggerMiddleware
    {
        private readonly RequestDelegate _nextDelegate;

        private readonly object _lock = new object();
        private readonly string _path;
        private readonly IEnumerable<Type> _controllerTypes;
        private string _swaggerJson = null;
        private readonly SwaggerOwinSettings _settings;
        private readonly SwaggerJsonSchemaGenerator _schemaGenerator;

        /// <summary>Initializes a new instance of the <see cref="SwaggerMiddleware"/> class.</summary>
        /// <param name="nextDelegate">The next delegate.</param>
        /// <param name="path">The path.</param>
        /// <param name="controllerTypes">The controller types.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        public SwaggerMiddleware(RequestDelegate nextDelegate, string path, IEnumerable<Type> controllerTypes, SwaggerOwinSettings settings, SwaggerJsonSchemaGenerator schemaGenerator)
        {
            _nextDelegate = nextDelegate;
            _path = path;
            _controllerTypes = controllerTypes;
            _settings = settings;
            _schemaGenerator = schemaGenerator;
        }

        /// <summary>Invokes the specified context.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The task.</returns>
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.HasValue && string.Equals(context.Request.Path.Value.Trim('/'), _path.Trim('/'), StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync(GenerateSwagger(context));
            }
            else
                await _nextDelegate(context);
        }

        /// <summary>Generates the Swagger specification.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The Swagger specification.</returns>
        protected virtual string GenerateSwagger(HttpContext context)
        {
            if (_swaggerJson == null)
            {
                lock (_lock)
                {
                    if (_swaggerJson == null)
                    {
                        try
                        {
                            var generator = new WebApiToSwaggerGenerator(_settings, _schemaGenerator);
                            var document = generator.GenerateForControllers(_controllerTypes);

                            document.Host = context.Request.Host.Value ?? "";
                            document.Schemes.Add(context.Request.Scheme == "http" ? SwaggerSchema.Http : SwaggerSchema.Https);
                            document.BasePath = context.Request.PathBase.Value?.Substring(0, context.Request.PathBase.Value.Length - _settings.MiddlewareBasePath?.Length ?? 0) ?? "";

                            _settings.PostProcess?.Invoke(document);
                            _swaggerJson = document.ToJson();
                        }
                        catch (Exception exception)
                        {
                            _swaggerJson = exception.ToString();
                        }
                    }
                }
            }

            return _swaggerJson;
        }
    }
}