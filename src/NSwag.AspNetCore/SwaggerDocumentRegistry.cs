//-----------------------------------------------------------------------
// <copyright file="DocumentRegistry.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.SwaggerGeneration;
using NSwag.SwaggerGeneration.AspNetCore;
using System;
using System.Collections.Generic;

namespace NSwag.AspNetCore
{
    /// <summary>Registry with Swagger document generators.</summary>
    public class SwaggerDocumentRegistry
    {
        private readonly Dictionary<string, RegisteredDocument> _documents;

        public SwaggerDocumentRegistry()
        {
            _documents = new Dictionary<string, RegisteredDocument>(StringComparer.Ordinal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentName"></param>
        /// <param name="configure"></param>
        /// <param name="schemaGenerator"></param>
        /// <returns></returns>
        public SwaggerDocumentRegistry AddDocument(
            string documentName,
            Action<AspNetCoreToSwaggerGeneratorSettings> configure = null,
            SwaggerJsonSchemaGenerator schemaGenerator = null)
        {
            var settings = new AspNetCoreToSwaggerGeneratorSettings();
            configure?.Invoke(settings);

            schemaGenerator = schemaGenerator ?? new SwaggerJsonSchemaGenerator(settings);
            this[documentName] = RegisteredDocument.CreateAspNetCoreGeneratorDocument(settings, schemaGenerator, null);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configure"></param>
        /// <param name="schemaGenerator"></param>
        /// <returns></returns>
        public SwaggerDocumentRegistry AddDocument(
            Action<AspNetCoreToSwaggerGeneratorSettings> configure = null,
            SwaggerJsonSchemaGenerator schemaGenerator = null)
        {
            return AddDocument("v1", configure, schemaGenerator);
        }


        internal RegisteredDocument this[string documentName]
        {
            get
            {
                if (documentName == null)
                {
                    throw new ArgumentNullException(nameof(documentName));
                }

                _documents.TryGetValue(documentName, out var document);
                return document;
            }

            set
            {
                if (documentName == null)
                {
                    throw new ArgumentNullException(nameof(documentName));
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _documents[documentName] = value;
            }
        }
    }
}
