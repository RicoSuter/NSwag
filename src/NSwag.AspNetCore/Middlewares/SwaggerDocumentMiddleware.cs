//-----------------------------------------------------------------------
// <copyright file="SwaggerMiddleware.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NSwag.AspNetCore.Middlewares
{
    /// <summary>Generates a Swagger specification on a given path.</summary>
    public class SwaggerDocumentMiddleware
    {
        private readonly RequestDelegate _nextDelegate;
        private readonly string _documentName;
        private readonly string _path;
        private readonly IApiDescriptionGroupCollectionProvider _apiDescriptionGroupCollectionProvider;
        private readonly SwaggerDocumentProvider _documentProvider;
        private readonly SwaggerDocumentMiddlewareSettings _settings;

        private int _version;
        private readonly object _documentsCacheLock = new object();
        private readonly Dictionary<string, Tuple<string, Exception, DateTimeOffset>> _documentsCache
            = new Dictionary<string, Tuple<string, Exception, DateTimeOffset>>();

        /// <summary>Initializes a new instance of the <see cref="WebApiToSwaggerMiddleware"/> class.</summary>
        /// <param name="nextDelegate">The next delegate.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="documentName">The document name.</param>
        /// <param name="path">The document path.</param>
        /// <param name="settings">The settings.</param>
        public SwaggerDocumentMiddleware(RequestDelegate nextDelegate, IServiceProvider serviceProvider, string documentName, string path, SwaggerDocumentMiddlewareSettings settings)
        {
            _nextDelegate = nextDelegate;

            _documentName = documentName;
            _path = path;

            _apiDescriptionGroupCollectionProvider = serviceProvider.GetService<IApiDescriptionGroupCollectionProvider>() ??
                throw new InvalidOperationException("API Explorer not registered in DI.");
            _documentProvider = serviceProvider.GetService<SwaggerDocumentProvider>() ??
                throw new InvalidOperationException("The NSwag DI services are not registered: Call " + nameof(NSwagServiceCollectionExtensions.AddSwaggerDocument) + "() in ConfigureServices().");

            _settings = settings;
        }

        /// <summary>Invokes the specified context.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The task.</returns>
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.HasValue && string.Equals(context.Request.Path.Value.Trim('/'), _path.Trim('/'), StringComparison.OrdinalIgnoreCase))
            {
                var schemaJson = await GetDocumentAsync(context);
                context.Response.StatusCode = 200;
                context.Response.Headers["Content-Type"] = "application/json; charset=utf-8";
                await context.Response.WriteAsync(schemaJson);
            }
            else
            {
                await _nextDelegate(context);
            }
        }

        /// <summary>Generates or gets the cached Swagger specification.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The Swagger specification.</returns>
        protected virtual async Task<string> GetDocumentAsync(HttpContext context)
        {
            var documentKey = _settings.CreateDocumentCacheKey?.Invoke(context.Request) ?? string.Empty;

            Tuple<string, Exception, DateTimeOffset> document;
            lock (_documentsCacheLock)
            {
                _documentsCache.TryGetValue(documentKey, out document);
            }

            if (document?.Item2 != null &&
                document.Item3 + _settings.ExceptionCacheTime > DateTimeOffset.UtcNow)
            {
                throw document.Item2;
            }

            var apiDescriptionGroups = _apiDescriptionGroupCollectionProvider.ApiDescriptionGroups;
            if (apiDescriptionGroups.Version == Volatile.Read(ref _version) &&
                document?.Item1 != null)
            {
                return document.Item1;
            }

            try
            {
                var json = (await GenerateDocumentAsync(context)).ToJson();
                _version = apiDescriptionGroups.Version;

                lock (_documentsCacheLock)
                {
                    _documentsCache[documentKey] = new Tuple<string, Exception, DateTimeOffset>(
                        json, null, DateTimeOffset.UtcNow);
                }

                return json;
            }
            catch (Exception exception)
            {
                lock (_documentsCacheLock)
                {
                    _documentsCache[documentKey] = new Tuple<string, Exception, DateTimeOffset>(
                        null, exception, DateTimeOffset.UtcNow);
                }

                throw;
            }
        }

        /// <summary>Generates the Swagger specification.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The Swagger specification.</returns>
        protected virtual async Task<SwaggerDocument> GenerateDocumentAsync(HttpContext context)
        {
            var document = await _documentProvider.GenerateAsync(_documentName);

            document.Host = context.Request.Host.Value ?? "";
            document.Schemes.Add(context.Request.Scheme == "http" ? SwaggerSchema.Http : SwaggerSchema.Https);
            document.BasePath = context.Request.PathBase.Value ?? "";

            _settings.PostProcess?.Invoke(document, context.Request);

            return document;
        }
    }
}
