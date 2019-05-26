//-----------------------------------------------------------------------
// <copyright file="SecurityDefinitionAppender.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NSwag.Generation.Processors.Contexts;

namespace NSwag.Generation.Processors.Security
{
    /// <summary>Appends the OAuth2 security scheme to the document's security definitions.</summary>
    public class SecurityDefinitionAppender : IDocumentProcessor
    {
        private readonly string _name;
        private readonly IEnumerable<string> _scopeNames;
        private readonly OpenApiSecurityScheme _swaggerSecurityScheme;

        /// <summary>Initializes a new instance of the <see cref="SecurityDefinitionAppender" /> class where the security requirement must be manually added.</summary>
        /// <param name="name">The name/key of the security scheme/definition.</param>
        /// <param name="swaggerSecurityScheme">The Swagger security scheme.</param>
        [Obsolete("Use the constructor with scopeNames parameter instead.")]
        public SecurityDefinitionAppender(string name, OpenApiSecurityScheme swaggerSecurityScheme)
        {
            _name = name;
            _swaggerSecurityScheme = swaggerSecurityScheme;
        }

        /// <summary>Initializes a new instance of the <see cref="SecurityDefinitionAppender" /> class.</summary>
        /// <param name="name">The name/key of the security scheme/definition.</param>
        /// <param name="scopeNames">The scope names to add to as security requirement with the scheme name in the 'security' property (can be an empty list).</param>
        /// <param name="swaggerSecurityScheme">The Swagger security scheme.</param>
        public SecurityDefinitionAppender(string name, IEnumerable<string> scopeNames, OpenApiSecurityScheme swaggerSecurityScheme)
        {
            _name = name;
            _scopeNames = scopeNames ?? throw new ArgumentNullException(nameof(scopeNames));
            _swaggerSecurityScheme = swaggerSecurityScheme;
        }

        /// <summary>Processes the specified Swagger document.</summary>
        /// <param name="context"></param>
        public void Process(DocumentProcessorContext context)
        {
            context.Document.SecurityDefinitions[_name] = _swaggerSecurityScheme;

            if (_scopeNames != null)
            {
                if (context.Document.Security == null)
                {
                    context.Document.Security = new Collection<OpenApiSecurityRequirement>();
                }

                context.Document.Security.Add(new OpenApiSecurityRequirement
                {
                    { _name, _scopeNames }
                });
            }
        }
    }
}
