//-----------------------------------------------------------------------
// <copyright file="SecurityDefinitionAppender.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
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
        public SecurityDefinitionAppender(string name, OpenApiSecurityScheme swaggerSecurityScheme)
        {
            _name = name;
            _swaggerSecurityScheme = swaggerSecurityScheme;
        }

        /// <summary>Initializes a new instance of the <see cref="SecurityDefinitionAppender" /> class.</summary>
        /// <param name="name">The name/key of the security scheme/definition.</param>
        /// <param name="globalScopeNames">The global scope names to add to as security requirement with the scheme name in the document's 'security' property (can be an empty list).</param>
        /// <param name="swaggerSecurityScheme">The Swagger security scheme.</param>
        public SecurityDefinitionAppender(string name, IEnumerable<string> globalScopeNames, OpenApiSecurityScheme swaggerSecurityScheme)
        {
            _name = name;
            _scopeNames = globalScopeNames ?? throw new ArgumentNullException(nameof(globalScopeNames));
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
