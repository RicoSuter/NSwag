//-----------------------------------------------------------------------
// <copyright file="SwaggerResponse.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NJsonSchema;

namespace NSwag.CodeGeneration
{
    /// <summary>Extension methods for <see cref="JsonSchema4"/>.</summary>
    public static class SchemaExtensions
    {
        /// <summary>Gets a value indicating whether the response schema is an exception.</summary>
        public static bool InheritsSchema(this JsonSchema4 schema, JsonSchema4 baseSchema)
        {
            // TODO: Move to NJsonSchema
            return baseSchema != null && schema?.ActualSchema
                .AllInheritedSchemas.Concat(new List<JsonSchema4> { schema })
                .Any(s => s.ActualSchema == baseSchema.ActualSchema) == true;
        }
    }
}
