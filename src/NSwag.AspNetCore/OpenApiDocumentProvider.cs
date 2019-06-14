//-----------------------------------------------------------------------
// <copyright file="NSwagDocumentProvider.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Microsoft.Extensions.ApiDescription;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Generation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NSwag.AspNetCore
{
    internal class OpenApiDocumentProvider : IDocumentProvider, IOpenApiDocumentGenerator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEnumerable<OpenApiDocumentRegistration> _documents;

        public OpenApiDocumentProvider(IServiceProvider serviceProvider, IEnumerable<OpenApiDocumentRegistration> documents)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _documents = documents ?? throw new ArgumentNullException(nameof(documents));
        }

        public async Task<OpenApiDocument> GenerateAsync(string documentName)
        {
            if (documentName == null)
            {
                throw new ArgumentNullException(nameof(documentName));
            }

            foreach (var group in _documents.GroupBy(g => g.DocumentName))
            {
                if (group.Count() > 1)
                {
                    throw new ArgumentException("The OpenAPI/Swagger document '" + group.Key + "' registered multiple times: " +
                        "Explicitely set the DocumentName property in " +
                        nameof(NSwagServiceCollectionExtensions.AddSwaggerDocument) + "() or " +
                        nameof(NSwagServiceCollectionExtensions.AddOpenApiDocument) + "().");
                }
            }

            var document = _documents.SingleOrDefault(g => g.DocumentName == documentName);
            if (document?.Generator == null)
            {
                throw new InvalidOperationException($"No registered OpenAPI/Swagger document found for the document name '{documentName}'. " +
                    $"Add with the AddSwagger()/AddOpenApi() methods in ConfigureServices().");
            }

            return await document.Generator.GenerateAsync(_serviceProvider);
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
