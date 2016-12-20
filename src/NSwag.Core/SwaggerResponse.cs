//-----------------------------------------------------------------------
// <copyright file="SwaggerResponse.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NJsonSchema;

namespace NSwag
{
    /// <summary>The Swagger response.</summary>
    public class SwaggerResponse
    {
        /// <summary>Gets the parent <see cref="SwaggerOperation"/>.</summary>
        [JsonIgnore]
        public SwaggerOperation Parent { get; internal set; }

        /// <summary>Gets or sets the response's description.</summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; } = "";

        /// <summary>Gets or sets the response schema.</summary>
        [JsonProperty(PropertyName = "schema", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonSchema4 Schema { get; set; }

        /// <summary>Gets or sets the headers.</summary>
        [JsonProperty(PropertyName = "header", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SwaggerHeaders Headers { get; set; }

        /// <summary>Sets a value indicating whether the response can be null (use IsNullable() to get a parameter's nullability).</summary>
        /// <remarks>The Swagger spec does not support null in schemas, see https://github.com/OAI/OpenAPI-Specification/issues/229 </remarks>
        [JsonProperty(PropertyName = "x-nullable", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool? IsNullableRaw { internal get; set; }

        /// <summary>Gets the actual non-nullable response schema (either oneOf schema or the actual schema).</summary>
        [JsonIgnore]
        public JsonSchema4 ActualResponseSchema => GetActualResponseSchema();

        /// <summary>Get or set the schema less extensions (this can be used as vendor extensions as well) in response schema.</summary>
        [JsonExtensionData]
        public IDictionary<string, object> ExtensionData { get; set; }

        /// <summary>Gets a value indicating whether the response schema is an exception.</summary>
        public bool InheritsExceptionSchema(JsonSchema4 exceptionSchema)
        {
            return exceptionSchema != null && ActualResponseSchema?
                .AllInheritedSchemas.Concat(new List<JsonSchema4> { ActualResponseSchema })
                .Any(s => s.ActualSchema == exceptionSchema.ActualSchema) == true;
        }

        /// <summary>Determines whether the specified null handling is nullable.</summary>
        /// <param name="nullHandling">The null handling.</param>
        /// <returns>The result.</returns>
        public bool IsNullable(NullHandling nullHandling)
        {
            if (nullHandling == NullHandling.Swagger)
            {
                if (IsNullableRaw == null)
                    return false;

                return IsNullableRaw.Value;
            }

            return Schema?.ActualSchema.IsNullable(nullHandling) ?? false;
        }

        private JsonSchema4 GetActualResponseSchema()
        {
            if (Parent?.Produces?.Contains("application/octet-stream") == true)
                return new JsonSchema4 { Type = JsonObjectType.File };

            return Schema?.ActualSchema;
        }
    }
}
