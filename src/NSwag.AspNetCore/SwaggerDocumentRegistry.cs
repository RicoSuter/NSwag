//-----------------------------------------------------------------------
// <copyright file="DocumentRegistry.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using NSwag.SwaggerGeneration;
using System;
using System.Collections.Generic;

namespace NSwag.AspNetCore
{
    /// <summary>Registry with Swagger document generators.</summary>
    public class SwaggerDocumentRegistry : ISwaggerDocumentBuilder
    {
        private readonly Dictionary<string, ISwaggerGenerator> _documents;

        /// <summary>Initializes a new instance of the <see cref="SwaggerDocumentRegistry"/> class.</summary>
        public SwaggerDocumentRegistry()
        {
            _documents = new Dictionary<string, ISwaggerGenerator>(StringComparer.Ordinal);
        }

        /// <summary>Adds a document to the registry.</summary>
        /// <param name="documentName">The document name.</param>
        /// <param name="swaggerGenerator">The Swagger generator.</param>
        /// <returns>The registry.</returns>
        public SwaggerDocumentRegistry AddDocument(string documentName, ISwaggerGenerator swaggerGenerator)
        {
            if (_documents.ContainsKey(documentName))
            {
                throw new ArgumentException("The OpenAPI/Swagger document '" + documentName + "' is already registered: " +
                    "Explicitely set the DocumentName property in " +
                    nameof(NSwagServiceCollectionExtensions.AddSwaggerDocument) + "() or " +
                    nameof(NSwagServiceCollectionExtensions.AddOpenApiDocument) + "().");
            }

            _documents[documentName] = swaggerGenerator;
            return this;
        }

        /// <summary>Gets a dictionary with all registered documents.</summary>
        public IReadOnlyDictionary<string, ISwaggerGenerator> Documents => _documents;
    }
}
