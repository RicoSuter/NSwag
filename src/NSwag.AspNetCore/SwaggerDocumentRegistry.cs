//-----------------------------------------------------------------------
// <copyright file="DocumentRegistry.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.AspNetCore.Documents;
using System;
using System.Collections.Generic;

namespace NSwag.AspNetCore
{
    /// <summary>Registry with Swagger document generators.</summary>
    public class SwaggerDocumentRegistry
    {
        private readonly Dictionary<string, ISwaggerDocument> _documents;

        /// <summary>Initializes a new instance of the <see cref="SwaggerDocumentRegistry"/> class.</summary>
        public SwaggerDocumentRegistry()
        {
            _documents = new Dictionary<string, ISwaggerDocument>(StringComparer.Ordinal);
        }

        /// <summary>Adds a document to the registry.</summary>
        /// <param name="configure">The configure action.</param>
        /// <returns>The registry.</returns>
        public SwaggerDocumentRegistry AddDocument(Action<AspNetCoreToSwaggerDocument> configure = null)
        {
            return AddDocument<AspNetCoreToSwaggerDocument>("v1", configure);
        }

        /// <summary>Adds a document to the registry.</summary>
        /// <param name="documentName">The document name.</param>
        /// <param name="configure">The configure action.</param>
        /// <returns>The registry.</returns>
        public SwaggerDocumentRegistry AddDocument(string documentName, Action<AspNetCoreToSwaggerDocument> configure = null)
        {
            return AddDocument<AspNetCoreToSwaggerDocument>(documentName, configure);
        }

        /// <summary>Adds a document to the registry.</summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="documentName">The document name.</param>
        /// <param name="configure">The configure action.</param>
        /// <returns>The registry.</returns>
        public SwaggerDocumentRegistry AddDocument<TDocument>(string documentName, Action<TDocument> configure = null)
            where TDocument : ISwaggerDocument, new()
        {
            var settings = new TDocument();
            configure?.Invoke(settings);
            _documents[documentName] = settings;
            return this;
        }

        /// <summary>Gets a dictionary with all registered documents.</summary>
        public IReadOnlyDictionary<string, ISwaggerDocument> Documents => _documents;
    }
}
