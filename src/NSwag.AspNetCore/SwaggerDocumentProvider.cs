//-----------------------------------------------------------------------
// <copyright file="NSwagDocumentProvider.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Microsoft.Extensions.ApiDescription;
using NSwag.SwaggerGeneration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NSwag.AspNetCore
{
    internal class SwaggerDocumentProvider : IDocumentProvider, ISwaggerDocumentProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly SwaggerDocumentRegistry _registry;

        public SwaggerDocumentProvider(IServiceProvider serviceProvider, SwaggerDocumentRegistry registry)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        public async Task<SwaggerDocument> GenerateAsync(string documentName)
        {
            if (documentName == null)
            {
                throw new ArgumentNullException(nameof(documentName));
            }

            _registry.Documents.TryGetValue(documentName, out var settings);
            if (settings == null)
            {
                throw new InvalidOperationException($"No registered OpenAPI/Swagger document found for the document name '{documentName}'. " +
                    $"Add with the AddSwagger()/AddOpenApi() methods in ConfigureServices().");
            }

            return await settings.GenerateAsync(_serviceProvider);
        }

        // Called by the Microsoft.Extensions.ApiDescription tool
        async Task IDocumentProvider.GenerateAsync(string documentName, TextWriter writer)
        {
            if (documentName == null)
            {
                throw new ArgumentNullException(nameof(documentName));
            }

            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            var document = await GenerateAsync(documentName);

            var json = document.ToJson();
            await writer.WriteAsync(json);
        }
    }
}
