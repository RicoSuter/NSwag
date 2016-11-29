//-----------------------------------------------------------------------
// <copyright file="SwaggerDocumentSchemaDefinitionAppender.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using NJsonSchema;

namespace NSwag
{
    /// <summary>Appends a JSON Schema to the Definitions of a Swagger document.</summary>
    public class SwaggerDocumentSchemaDefinitionAppender : ISchemaDefinitionAppender
    {
        private readonly SwaggerDocument _document;
        private readonly ITypeNameGenerator _typeNameGenerator;

        /// <summary>Initializes a new instance of the <see cref="SwaggerDocumentSchemaDefinitionAppender" /> class.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="typeNameGenerator">The type name generator.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document"/> is <see langword="null"/></exception>
        public SwaggerDocumentSchemaDefinitionAppender(SwaggerDocument document, ITypeNameGenerator typeNameGenerator)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            _document = document;
            _typeNameGenerator = typeNameGenerator;
        }

        /// <summary>Tries to set the root of the appender.</summary>
        /// <param name="rootObject">The root object.</param>
        /// <returns>true when the root was not set before.</returns>
        public bool TrySetRoot(object rootObject)
        {
            return false;
        }

        /// <summary>Appends the schema to the root object.</summary>
        /// <param name="schema">The schema to append.</param>
        /// <param name="typeNameHint">The type name hint.</param>
        public void AppendSchema(JsonSchema4 schema, string typeNameHint)
        {
            schema = schema.ActualSchema;
            if (!_document.Definitions.Values.Contains(schema))
            {
                var typeName = schema.GetTypeName(_typeNameGenerator, typeNameHint);
                if (!string.IsNullOrEmpty(typeName) && !_document.Definitions.ContainsKey(typeName))
                    _document.Definitions[typeName] = schema;
                else
                    _document.Definitions["ref_" + Guid.NewGuid().ToString().Replace("-", "_")] = schema;
            }
        }
    }
}