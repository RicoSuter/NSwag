//-----------------------------------------------------------------------
// <copyright file="SwaggerMiddleware.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace NSwag.AspNetCore
{
    internal class SwaggerMiddleware
    {
        private readonly RequestDelegate _nextDelegate;

        private readonly object _lock = new object();
        private readonly string _path;
        private readonly IEnumerable<Assembly> _webApiAssemblies;
        private string _swaggerJson = null;

        public SwaggerMiddleware(RequestDelegate nextDelegate, string path, IEnumerable<Assembly> webApiAssemblies/*, WebApiToSwaggerGeneratorSettings settings*/)
        {
            _nextDelegate = nextDelegate;
            _path = path;
            _webApiAssemblies = webApiAssemblies;
            //_settings = settings;
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
            return "foobar";
            //if (_swaggerJson == null)
            //{
            //    lock (_lock)
            //    {
            //        if (_swaggerJson == null)
            //        {
            //            var generator = new WebApiToSwaggerGenerator(_settings);
            //            var controllers = _webApiAssemblies.SelectMany(WebApiToSwaggerGenerator.GetControllerClasses);
            //            var service = generator.GenerateForControllers(controllers);

            //            service.Host = context.Request.Host.Value;
            //            service.Schemes.Add(context.Request.Uri.Scheme == "http" ? SwaggerSchema.Http : SwaggerSchema.Https);

            //            _swaggerJson = service.ToJson();
            //        }
            //    }
            //}

            return _swaggerJson;
        }
    }
}