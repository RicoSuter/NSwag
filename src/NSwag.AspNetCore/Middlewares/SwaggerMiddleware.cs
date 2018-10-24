//-----------------------------------------------------------------------
// <copyright file="SwaggerMiddleware.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Internal;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace NSwag.AspNetCore.Middlewares
{
    internal class SwaggerMiddleware
    {
        private const string DocumentNameEntry = "documentName";
        private readonly SwaggerDocumentProvider _documentProvider;
        private readonly TemplateMatcher _matcher;
        private readonly RequestDelegate _nextDelegate;
        private readonly SwaggerMiddlewareOptions _options;

        private string _failedDocumentName;
        private Exception _documentException;
        private DateTimeOffset _exceptionTimestamp;

        public SwaggerMiddleware(
            RequestDelegate nextDelegate,
            SwaggerDocumentProvider documentProvider,
            ObjectPool<UriBuildingContext> pool,
            IOptions<SwaggerMiddlewareOptions> options)
        {
            _documentProvider = documentProvider;

            _options = options.Value;
            var path = _options.SwaggerRoute.StartsWith("/") ?
                _options.SwaggerRoute.Substring(1) :
                _options.SwaggerRoute;
            var template = TemplateParser.Parse(path);

            _matcher = new TemplateMatcher(template, defaults: null);
            _nextDelegate = nextDelegate;
        }

        public async Task Invoke(HttpContext context)
        {
            var values = new RouteValueDictionary();
            if (_matcher.TryMatch(context.Request.Path, values))
            {
                var documentName = "v1";
                if (values.TryGetValue(DocumentNameEntry, out var document))
                {
                    documentName = document as string;
                    if (documentName == null)
                    {
                        throw new InvalidOperationException($"Unexpected null '{DocumentNameEntry}' entry in " +
                            $"{nameof(RouteValueDictionary)}.");
                    }
                }

                var documentString = await GenerateAsync(documentName);
                context.Response.StatusCode = 200;
                context.Response.Headers["Content-Type"] = "application/json; charset=utf-8";
                await context.Response.WriteAsync(documentString);
            }
            else
            {
                await _nextDelegate(context);
            }
        }

        protected virtual async Task<string> GenerateAsync(string documentName)
        {
            var now = DateTimeOffset.UtcNow;
            if (_documentException != null &&
                _exceptionTimestamp + _options.ExceptionCacheTime > now &&
                string.Equals(_failedDocumentName, documentName, StringComparison.Ordinal))
            {
                throw _documentException;
            }

            try
            {
                var document = await _documentProvider.GenerateAsync(documentName);
                _options.PostProcess?.Invoke(document);
                return document.ToJson();
            }
            catch (Exception exception)
            {
                _documentException = exception;
                _exceptionTimestamp = now;
                _failedDocumentName = documentName;

                throw _documentException;
            }
        }
    }
}
