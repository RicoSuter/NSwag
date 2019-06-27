//-----------------------------------------------------------------------
// <copyright file="OperationSecurityScopeProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag;
using NSwag.Generation;
using NSwag.Generation.Processors.Security;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions for the <see cref="OpenApiDocumentGeneratorSettings"/>.
    /// </summary>
    public static class NSwagSwaggerGeneratorSettingsExtensions
    {
        /// <summary>Appends the OAuth2 security scheme and requirement to the document's security definitions.</summary>
        /// <remarks>Adds a <see cref="SecurityDefinitionAppender"/> document processor with the given arguments.</remarks>
        /// <param name="settings">The settings.</param>
        /// <param name="name">The name/key of the security scheme/definition.</param>
        /// <param name="scopeNames">The scope names to add to as security requirement with the scheme name in the 'security' property (can be an empty list).</param>
        /// <param name="swaggerSecurityScheme">The Swagger security scheme.</param>
        public static OpenApiDocumentGeneratorSettings AddSecurity(this OpenApiDocumentGeneratorSettings settings, string name, IEnumerable<string> scopeNames, OpenApiSecurityScheme swaggerSecurityScheme)
        {
            settings.DocumentProcessors.Add(new SecurityDefinitionAppender(name, scopeNames, swaggerSecurityScheme));
            return settings;
        }
    }
}
