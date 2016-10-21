//-----------------------------------------------------------------------
// <copyright file="SwaggerServiceSchemaDefinitionAppender.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using NJsonSchema;

namespace NSwag
{
    /// <summary>Appends a JSON Schema to the Definitions of a Swagger service.</summary>
    public class SwaggerServiceSchemaDefinitionAppender : ISchemaDefinitionAppender
    {
        private readonly SwaggerService _service;
        private readonly ITypeNameGenerator _typeNameGenerator;

        /// <summary>Initializes a new instance of the <see cref="SwaggerServiceSchemaDefinitionAppender" /> class.</summary>
        /// <param name="service">The service.</param>
        /// <param name="typeNameGenerator">The type name generator.</param>
        public SwaggerServiceSchemaDefinitionAppender(SwaggerService service, ITypeNameGenerator typeNameGenerator)
        {
            _service = service;
            _typeNameGenerator = typeNameGenerator; 
        }

        /// <summary>Gets or sets the root object to append schemas to.</summary>
        public object RootObject { get; set; }

        /// <summary>Appends the schema to the root object.</summary>
        /// <param name="objectToAppend">The object to append.</param>
        public void Append(JsonSchema4 objectToAppend)
        {
            var typeName = objectToAppend.GetTypeName(_typeNameGenerator, string.Empty); 
            if (!string.IsNullOrEmpty(typeName) && !_service.Definitions.ContainsKey(typeName))
                _service.Definitions[typeName] = objectToAppend;
            else
                _service.Definitions["ref_" + Guid.NewGuid().ToString().Replace("-", "_")] = objectToAppend;
        }
    }
}