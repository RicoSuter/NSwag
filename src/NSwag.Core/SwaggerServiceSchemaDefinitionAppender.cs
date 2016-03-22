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
        /// <summary>Appends the schema to the root object.</summary>
        /// <param name="root">The root object.</param>
        /// <param name="objectToAppend">The object to append.</param>
        /// <exception cref="InvalidOperationException">Could not find the JSON path of a child object.</exception>
        public void Append(object root, JsonSchema4 objectToAppend)
        {
            var rootService = root as SwaggerService;
            if (rootService != null && objectToAppend != null)
                rootService.Definitions["ref_" + Guid.NewGuid()] = objectToAppend;
            else
                throw new InvalidOperationException("Could not find the JSON path of a child object.");
        }
    }
}