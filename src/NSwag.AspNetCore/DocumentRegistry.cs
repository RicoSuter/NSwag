//-----------------------------------------------------------------------
// <copyright file="DocumentRegistry.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace NSwag.AspNetCore
{
    internal class DocumentRegistry
    {
        private readonly Dictionary<string, RegisteredDocument> _documents;

        public DocumentRegistry()
        {
            _documents = new Dictionary<string, RegisteredDocument>(StringComparer.Ordinal);
        }

        public RegisteredDocument this[string documentName]
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
