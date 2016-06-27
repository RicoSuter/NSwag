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
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.AspNetCore
{
    internal class SwaggerMiddleware
    {
        private readonly RequestDelegate _nextDelegate;

        private readonly object _lock = new object();
        private readonly string _path;
        private readonly IEnumerable<Type> _controllerTypes;
        private string _swaggerJson = null;
        private readonly SwaggerOwinSettings _settings;

        public SwaggerMiddleware(RequestDelegate nextDelegate, string path, IEnumerable<Type> controllerTypes, SwaggerOwinSettings settings)
        {
            _nextDelegate = nextDelegate;
            _path = path;
            _controllerTypes = controllerTypes;
            _settings = settings;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.Trim('/') == _path.Trim('/'))
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync(GenerateSwagger(context));
            }
            else
                await _nextDelegate(context);
        }

        private string GenerateSwagger(HttpContext context)
        {
            if (_swaggerJson == null)
            {
                lock (_lock)
                {
                    if (_swaggerJson == null)
                    {
                        var generator = new WebApiToSwaggerGenerator(_settings);
                        var service = generator.GenerateForControllers(_controllerTypes);
                        
                        _settings?.PostProcess(service);
                        _swaggerJson = service.ToJson();
                    }
                }
            }

            return _swaggerJson;
        }
    }
}