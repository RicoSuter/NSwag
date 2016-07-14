//-----------------------------------------------------------------------
// <copyright file="OAuth2SchemeBuilder.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors
{
    /// <summary>Appends the OAuth2 security scheme to the document's security definitions.</summary>
    public class OAuth2SchemeBuilder : IDocumentProcessor
    {
        private readonly SwaggerSecurityScheme _swaggerSecurityScheme;

        /// <summary>Initializes a new instance of the <see cref="OAuth2SchemeBuilder"/> class.</summary>
        /// <param name="swaggerSecurityScheme">The swagger security scheme.</param>
        public OAuth2SchemeBuilder(SwaggerSecurityScheme swaggerSecurityScheme)
        {
            _swaggerSecurityScheme = swaggerSecurityScheme;
        }

        /// <summary>Processes the specified Swagger document.</summary>
        /// <param name="document">The document.</param>
        public void Process(SwaggerService document)
        {
            document.SecurityDefinitions["oauth2"] = _swaggerSecurityScheme;
        }
    }
}