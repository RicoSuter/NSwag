//-----------------------------------------------------------------------
// <copyright file="NSwagDocumentProvider.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.ApiDescriptions;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Generation;

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
                        "Explicitly set the DocumentName property in " +
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

        // Called by the <c>dotnet-getdocument</c> tool from the Microsoft.Extensions.ApiDescription.Server package.
        IEnumerable<string> IDocumentProvider.GetDocumentNames()
        {
            // DocumentName may be null. But, if it is, cannot generate the registered document.
            return _documents
                .Where(document => document.DocumentName != null)
                .Select(document => document.DocumentName);
        }

        // Called by the <c>dotnet-getdocument</c> tool from the Microsoft.Extensions.ApiDescription.Server package.
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
