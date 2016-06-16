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
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.AspNet.Owin
{
    internal class SwaggerMiddleware : OwinMiddleware
    {
        private readonly object _lock = new object();
        private readonly string _path;
        private readonly WebApiToSwaggerGeneratorSettings _settings;
        private readonly IEnumerable<Type> _controllerTypes;
        private string _swaggerJson = null;

        public SwaggerMiddleware(OwinMiddleware next, string path, IEnumerable<Type> controllerTypes, WebApiToSwaggerGeneratorSettings settings)
            : base(next)
        {
            _path = path;
            _controllerTypes = controllerTypes;
            _settings = settings;
        }

        public override async Task Invoke(IOwinContext context)
        {
            var url = context.Request.Uri;
            if (url.PathAndQuery.Trim('/') == _path.Trim('/'))
            {
                context.Response.StatusCode = 200;
                context.Response.Write(GenerateSwagger(context));
            }
            else
                await Next.Invoke(context);
        }

        private string GenerateSwagger(IOwinContext context)
        {
            if (_swaggerJson == null)
            {
                lock (_lock)
                {
                    if (_swaggerJson == null)
                    {
                        var generator = new WebApiToSwaggerGenerator(_settings);
                        var service = generator.GenerateForControllers(_controllerTypes);

                        service.Host = context.Request.Host.Value;
                        service.Schemes.Add(context.Request.Uri.Scheme == "http" ? SwaggerSchema.Http : SwaggerSchema.Https);

                        _swaggerJson = service.ToJson();
                    }
                }
            }

            return _swaggerJson;
        }
    }
}