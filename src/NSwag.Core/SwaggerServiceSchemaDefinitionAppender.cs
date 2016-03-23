//-----------------------------------------------------------------------
// <copyright file="SwaggerServiceSchemaDefinitionAppender.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;

namespace NSwag
{
    /// <summary>Appends a JSON Schema to the Definitions of a Swagger service.</summary>
    public class SwaggerServiceSchemaDefinitionAppender : ISchemaDefinitionAppender
    {
        private readonly SwaggerService _service;

        public SwaggerServiceSchemaDefinitionAppender(SwaggerService service)
        {
            _service = service; 
        }

        /// <summary>Appends the schema to the root object.</summary>
        /// <param name="root">The root object.</param>
        /// <param name="objectToAppend">The object to append.</param>
        public void Append(object root, JsonSchema4 objectToAppend)
        {
            // Do nothing because all generated schemes from the ISchemaResolver are added to the Swagger service's definitions
        }
    }
}