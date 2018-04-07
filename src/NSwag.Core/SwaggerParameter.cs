//-----------------------------------------------------------------------
// <copyright file="SwaggerParameter.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NJsonSchema;

namespace NSwag
{
    /// <summary>Describes an operation parameter. </summary>
    public class SwaggerParameter : JsonSchema4
    {
        private string _name;
        private SwaggerParameterKind _kind;
        private bool _isRequired = false;
        private JsonSchema4 _schema;
        private IDictionary<string, OpenApiExample> _examples;

        [JsonIgnore]
        internal SwaggerOperation ParentOperation => Parent as SwaggerOperation;

        /// <summary>Gets or sets the name.</summary>
        [JsonProperty(PropertyName = "name", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                ParentOperation?.UpdateRequestBody(this);
            }
        }

        /// <summary>Gets or sets the kind of the parameter.</summary>
        [JsonProperty(PropertyName = "in", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SwaggerParameterKind Kind
        {
            get => _kind;
            set
            {
                _kind = value;
                ParentOperation?.UpdateRequestBody(this);
            }
        }

        /// <summary>Gets or sets a value indicating whether the parameter is required (default: false).</summary>
        [JsonProperty(PropertyName = "required", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool IsRequired
        {
            get => _isRequired;
            set
            {
                _isRequired = value;
                ParentOperation?.UpdateRequestBody(this);
            }
        }

        /// <summary>Gets or sets a value indicating whether passing empty-valued parameters is allowed (default: false).</summary>
        [JsonProperty(PropertyName = "allowEmptyValue", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool AllowEmptyValue { get; set; }

        /// <summary>Gets or sets the schema which is only available when <see cref="Kind"/> == body.</summary>
        [JsonProperty(PropertyName = "schema", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public JsonSchema4 Schema
        {
            get => _schema;
            set
            {
                _schema = value;
                ParentOperation?.UpdateRequestBody(this);
            }
        }

        /// <summary>Gets or sets the description. </summary>
        [Newtonsoft.Json.JsonProperty("description", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public override string Description
        {
            get => base.Description;
            set
            {
                base.Description = value;
                ParentOperation?.UpdateRequestBody(this);
            }
        }

        /// <summary>Gets or sets the custom schema which is used when <see cref="Kind"/> != body.</summary>
        [JsonProperty(PropertyName = "x-schema", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public JsonSchema4 CustomSchema { get; set; }

        /// <summary>Gets the actual schema, either the parameter schema itself (or its reference) or the <see cref="Schema"/> property when <see cref="Kind"/> == body.</summary>
        /// <exception cref="InvalidOperationException" accessor="get">The schema reference path is not resolved.</exception>
        [JsonIgnore]
        public override JsonSchema4 ActualSchema
        {
            get
            {
                if (Reference is SwaggerParameter parameter)
                    return parameter.ActualSchema;
                else
                    return Schema?.ActualSchema ?? CustomSchema?.ActualSchema ?? base.ActualSchema;
            }
        }

        /// <summary>Gets the actual parameter.</summary>
        [JsonIgnore]
        public SwaggerParameter ActualParameter => Reference is SwaggerParameter ? (SwaggerParameter)Reference : this;

        /// <summary>Gets or sets the format of the array if type array is used.</summary>
        [JsonProperty(PropertyName = "collectionFormat", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SwaggerParameterCollectionFormat CollectionFormat { get; set; }

        /// <summary>Gets or sets the headers (OpenAPI only).</summary>
        [JsonProperty(PropertyName = "examples", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, OpenApiExample> Examples
        {
            get => _examples;
            set
            {
                _examples = value;
                ParentOperation?.UpdateRequestBody(this);
            }
        }

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
            else if (schemaType == SchemaType.OpenApi3 && IsNullableRaw.HasValue)
                return IsNullableRaw.Value;

            return base.IsNullable(schemaType);
        }

        /// <summary>Gets a value indicating whether this is an XML body parameter.</summary>
        [JsonIgnore]
        public bool IsXmlBodyParameter => Kind == SwaggerParameterKind.Body && (Parent as SwaggerOperation)?.ActualConsumes?.FirstOrDefault() == "application/xml";

        /// <summary>Gets a value indicating whether this is an binary body parameter.</summary>
        [JsonIgnore]
        public bool IsBinaryBodyParameter => Kind == SwaggerParameterKind.Body && (Parent as SwaggerOperation)?.ActualConsumes?.FirstOrDefault() == "application/octet-stream";
    }
}