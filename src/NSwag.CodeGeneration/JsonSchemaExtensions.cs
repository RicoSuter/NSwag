//-----------------------------------------------------------------------
// <copyright file="ClientGeneratorBase.cs" company="NSwag">
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
    /// <summary>JSON Schema extensions.</summary>
    public static class JsonSchema4Extensions
    {
        /// <summary>Checks whether the schema uses an object schema somewhere (i.e. does it require a DTO class).</summary>
        /// <param name="schema">The schema.</param>
        /// <returns>true or false</returns>
        public static bool UsesComplexObjectSchema(this JsonSchema schema)
        {
            return UsesComplexObjectSchema(schema, new List<JsonSchema>());
        }

        private static bool UsesComplexObjectSchema(this JsonSchema schema, List<JsonSchema> checkedSchemas)
        {
            schema = schema.ActualTypeSchema;

            if (checkedSchemas.Contains(schema))
                return false;
            checkedSchemas.Add(schema);

            if (schema.IsDictionary)
                return schema.AdditionalPropertiesSchema?.UsesComplexObjectSchema(checkedSchemas) == true;

            if (schema.Type.HasFlag(JsonObjectType.Array))
            {
                return schema.Item?.UsesComplexObjectSchema(checkedSchemas) == true ||
                       schema.Items?.Any(i => i.UsesComplexObjectSchema(checkedSchemas)) == true ||
                       schema.AdditionalItemsSchema?.UsesComplexObjectSchema(checkedSchemas) == true;
            }

            if (schema.Type.HasFlag(JsonObjectType.Object))
                return !schema.IsAnyType;

            return false;
        }
    }
}