//-----------------------------------------------------------------------
// <copyright file="SwaggerSchemaResolver.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using NJsonSchema;
using NJsonSchema.Generation;

namespace NSwag
{
    /// <summary>Appends a JSON Schema to the Definitions of a Swagger document.</summary>
    public class SwaggerSchemaResolver : JsonSchemaResolver
    {
        private readonly SwaggerDocument _document;
        private readonly ITypeNameGenerator _typeNameGenerator;

        /// <summary>Initializes a new instance of the <see cref="SwaggerSchemaResolver" /> class.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document" /> is <see langword="null" /></exception>
        public SwaggerSchemaResolver(SwaggerDocument document, JsonSchemaGeneratorSettings settings)
            : base(settings)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            _document = document;
            _typeNameGenerator = settings.TypeNameGenerator;
        }
        
        /// <summary>Appends the schema to the root object.</summary>
        /// <param name="schema">The schema to append.</param>
        /// <param name="typeNameHint">The type name hint.</param>
        public override void AppendSchema(JsonSchema4 schema, string typeNameHint)
        {
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