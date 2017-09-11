//-----------------------------------------------------------------------
// <copyright file="SwaggerParameter.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using Newtonsoft.Json;
using NJsonSchema;

namespace NSwag
{
    /// <summary>Describes an operation parameter. </summary>
    public class SwaggerParameter : JsonSchema4
    {
        /// <summary>Gets the parent operation.</summary>
        [JsonIgnore]
        public SwaggerOperation Parent { get; internal set; }

        /// <summary>Gets or sets the name.</summary>
        [JsonProperty(PropertyName = "name", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Name { get; set; }

        /// <summary>Gets or sets the kind of the parameter.</summary>
        [JsonProperty(PropertyName = "in", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SwaggerParameterKind Kind { get; set; }

        /// <summary>Gets or sets a value indicating whether the parameter is required (default: false).</summary>
        [JsonProperty(PropertyName = "required", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool IsRequired { get; set; } = false;

        /// <summary>Gets or sets a value indicating whether passing empty-valued parameters is allowed (default: false).</summary>
        [JsonProperty(PropertyName = "allowEmptyValue", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool AllowEmptyValue { get; set; }

        /// <summary>Gets or sets the schema which is only available when <see cref="Kind"/> == body.</summary>
        [JsonProperty(PropertyName = "schema", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public JsonSchema4 Schema { get; set; }

        /// <summary>Gets or sets the custom schema which is used when <see cref="Kind"/> != body.</summary>
        [JsonProperty(PropertyName = "x-schema", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public JsonSchema4 CustomSchema { get; set; }

        /// <summary>Sets a value indicating whether the parameter can be null (use IsNullable() to get a parameter's nullability).</summary>
        /// <remarks>The Swagger spec does not support null in schemas, see https://github.com/OAI/OpenAPI-Specification/issues/229 </remarks>
        [JsonProperty(PropertyName = "x-nullable", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool? IsNullableRaw { internal get; set; }

        /// <summary>Gets the actual schema, either the parameter schema itself (or its reference) or the <see cref="Schema"/> property when <see cref="Kind"/> == body.</summary>
        /// <exception cref="InvalidOperationException" accessor="get">The schema reference path is not resolved.</exception>
        [JsonIgnore]
        public override JsonSchema4 ActualSchema => Schema?.ActualSchema ?? CustomSchema?.ActualSchema ?? base.ActualSchema;

        /// <summary>Gets or sets the format of the array if type array is used.</summary>
        [JsonProperty(PropertyName = "collectionFormat", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SwaggerParameterCollectionFormat CollectionFormat { get; set; }

        /// <summary>Gets a value indicating whether the validated data can be null.</summary>
        /// <param name="schemaType">The schema type.</param>
        /// <returns>The result.</returns>
        public override bool IsNullable(SchemaType schemaType)
        {
            if (schemaType == SchemaType.Swagger2)
            {
                if (IsNullableRaw == null)
                    return IsRequired == false;

                return IsNullableRaw.Value;
            }

            return base.IsNullable(schemaType);
        }

        /// <summary>Gets a value indicating whether this is an XML body parameter.</summary>
        [JsonIgnore]
        public bool IsXmlBodyParameter => Kind == SwaggerParameterKind.Body && Parent.ActualConsumes?.FirstOrDefault() == "application/xml";

        /// <summary>Gets a value indicating whether this is an binary body parameter.</summary>
        [JsonIgnore]
        public bool IsBinaryBodyParameter => Kind == SwaggerParameterKind.Body && Parent.ActualConsumes?.FirstOrDefault() == "application/octet-stream";
    }
}