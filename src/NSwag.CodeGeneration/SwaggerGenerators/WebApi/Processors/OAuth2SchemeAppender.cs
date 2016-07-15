//-----------------------------------------------------------------------
// <copyright file="OAuth2SchemeAppender.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors
{
    /// <summary>Appends the OAuth2 security scheme to the document's security definitions.</summary>
    public class OAuth2SchemeAppender : IDocumentProcessor
    {
        private readonly string _name;
        private readonly SwaggerSecurityScheme _swaggerSecurityScheme;

        /// <summary>Initializes a new instance of the <see cref="OAuth2SchemeAppender" /> class.</summary>
        /// <param name="name">The name of the schema.</param>
        /// <param name="swaggerSecurityScheme">The swagger security scheme.</param>
        public OAuth2SchemeAppender(string name, SwaggerSecurityScheme swaggerSecurityScheme)
        {
            _name = name; 
            _swaggerSecurityScheme = swaggerSecurityScheme;
        }

        /// <summary>Processes the specified Swagger document.</summary>
        /// <param name="document">The document.</param>
        public void Process(SwaggerService document)
        {
            _swaggerSecurityScheme.Type = SwaggerSecuritySchemeType.OAuth2;
            document.SecurityDefinitions[_name] = _swaggerSecurityScheme;
        }
    }
}