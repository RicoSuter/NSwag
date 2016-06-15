//-----------------------------------------------------------------------
// <copyright file="SwaggerMiddleware.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Owin;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.SwaggerUi.AspNet
{
    internal class SwaggerMiddleware : OwinMiddleware
    {
        private readonly object _lock = new object();
        private readonly string _path;
        private readonly WebApiToSwaggerGeneratorSettings _settings;
        private readonly IEnumerable<Assembly> _webApiAssemblies;
        private string _swaggerJson = null;

        public SwaggerMiddleware(OwinMiddleware next, string path, IEnumerable<Assembly> webApiAssemblies, WebApiToSwaggerGeneratorSettings settings)
            : base(next)
        {
            _path = path;
            _webApiAssemblies = webApiAssemblies;
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
                        var controllers = _webApiAssemblies.SelectMany(WebApiToSwaggerGenerator.GetControllerClasses);
                        var service = generator.GenerateForControllers(controllers);

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