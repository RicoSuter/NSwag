//-----------------------------------------------------------------------
// <copyright file="WebApiToSwaggerGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Infrastructure;

namespace NSwag.CodeGeneration.SwaggerGenerators
{
    /// <summary>Provides services to for Swagger generators like the creation of parameters and handling of schemas.</summary>
    public class SwaggerGenerator
    {
        private readonly JsonSchemaResolver _schemaResolver;
        private readonly JsonSchemaGenerator _schemaGenerator;
        private readonly JsonSchemaGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="SwaggerGenerator"/> class.</summary>
        /// <param name="schemaGenerator">The schema generator.</param>
        /// <param name="schemaGeneratorSettings">The schema generator settings.</param>
        /// <param name="schemaResolver">The schema resolver.</param>
        public SwaggerGenerator(JsonSchemaGenerator schemaGenerator, JsonSchemaGeneratorSettings schemaGeneratorSettings, JsonSchemaResolver schemaResolver)
        {
            _schemaResolver = schemaResolver;
            _schemaGenerator = schemaGenerator;
            _settings = schemaGeneratorSettings;
        }

        /// <summary>Creates a primitive parameter for the given parameter information reflection object.</summary>
        /// <param name="name">The name.</param>
        /// <param name="parameter">The parameter information.</param>
        /// <returns>The created parameter.</returns>
        public SwaggerParameter CreatePrimitiveParameter(string name, ParameterInfo parameter)
        {
            return CreatePrimitiveParameter(name, parameter.GetXmlDocumentation(), parameter.ParameterType, parameter.GetCustomAttributes().ToList());
        }
        
        /// <summary>Creates a path parameter for a given type.</summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <returns>The parameter.</returns>
        public SwaggerParameter CreatePathParameter(string parameterName, string parameterType)
        {
            var parameter = new SwaggerParameter();
            parameter.Name = parameterName;
            parameter.Kind = SwaggerParameterKind.Path;
            parameter.IsRequired = true;
            parameter.IsNullableRaw = false;

            if (parameterType == "guid")
            {
                parameter.Type = JsonObjectType.String;
                parameter.Format = JsonFormatStrings.Guid;
            }
            else if (parameterType == "int" || parameterType == "integer" || parameterType == "short" || parameterType == "long")
                parameter.Type = JsonObjectType.Integer;
            else if (parameterType == "number" || parameterType == "decimal" || parameterType == "double")
                parameter.Type = JsonObjectType.Number;
            else
                parameter.Type = JsonObjectType.String;

            return parameter;
        }

        /// <summary>Creates a primitive parameter for the given parameter information reflection object.</summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="parentAttributes">The parent attributes.</param>
        /// <returns></returns>
        public SwaggerParameter CreatePrimitiveParameter(string name, string description, Type parameterType, IList<Attribute> parentAttributes)
        {
            var typeDescription = JsonObjectTypeDescription.FromType(parameterType, parentAttributes, _settings.DefaultEnumHandling);

            SwaggerParameter operationParameter;
            if (typeDescription.IsEnum)
            {
                // TODO(incompatibility): We use "schema" even it is not allowed in non-body parameters
                parameterType = parameterType.Name == "Nullable`1" ? parameterType.GetGenericTypeArguments().Single() : parameterType;
                operationParameter = new SwaggerParameter
                {
                    Type = typeDescription.Type, // Used as fallback for generators which do not check the "schema" property
                    Schema = new JsonSchema4
                    {
                        SchemaReference = _schemaGenerator.Generate<JsonSchema4>(parameterType, parentAttributes, _schemaResolver)
                    }
                };
            }
            else
            {
                parameterType = typeDescription.Type.HasFlag(JsonObjectType.Object) ? typeof(string) : parameterType; // object types must be treated as string

                operationParameter = _schemaGenerator.Generate<SwaggerParameter>(parameterType, parentAttributes, _schemaResolver);
                _schemaGenerator.ApplyPropertyAnnotations(operationParameter, parameterType, parentAttributes, typeDescription);
            }

            operationParameter.Name = name;
            operationParameter.IsRequired = parentAttributes?.Any(a => a.GetType().Name == "RequiredAttribute") ?? false;
            operationParameter.IsNullableRaw = typeDescription.IsNullable;

            if (description != string.Empty)
                operationParameter.Description = description;

            return operationParameter;
        }

        /// <summary>Creates a primitive parameter for the given parameter information reflection object.</summary>
        /// <param name="name">The name.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        public SwaggerParameter CreateBodyParameter(string name, ParameterInfo parameter)
        {
            var isRequired = IsParameterRequired(parameter);

            var typeDescription = JsonObjectTypeDescription.FromType(parameter.ParameterType, parameter.GetCustomAttributes(), _settings.DefaultEnumHandling);
            var operationParameter = new SwaggerParameter
            {
                Name = name,
                Kind = SwaggerParameterKind.Body,
                IsRequired = isRequired,
                IsNullableRaw = typeDescription.IsNullable,
                Schema = GenerateAndAppendSchemaFromType(parameter.ParameterType, !isRequired, parameter.GetCustomAttributes()),
            };

            var description = parameter.GetXmlDocumentation();
            if (description != string.Empty)
                operationParameter.Description = description;

            return operationParameter;
        }

        /// <summary>Generates and appends a schema from a given type.</summary>
        /// <param name="type">The type.</param>
        /// <param name="mayBeNull">if set to <c>true</c> [may be null].</param>
        /// <param name="parentAttributes">The parent attributes.</param>
        /// <returns></returns>
        public JsonSchema4 GenerateAndAppendSchemaFromType(Type type, bool mayBeNull, IEnumerable<Attribute> parentAttributes)
        {
            if (type.Name == "Task`1")
                type = type.GenericTypeArguments[0];

            if (type.Name == "JsonResult`1")
                type = type.GenericTypeArguments[0];

            if (IsFileResponse(type))
                return new JsonSchema4 { Type = JsonObjectType.File };

            var typeDescription = JsonObjectTypeDescription.FromType(type, parentAttributes, _settings.DefaultEnumHandling);
            if (typeDescription.Type.HasFlag(JsonObjectType.Object) && !typeDescription.IsDictionary)
            {
                if (type == typeof(object))
                {
                    return new JsonSchema4
                    {
                        // IsNullable is directly set on SwaggerParameter or SwaggerResponse
                        Type = _settings.NullHandling == NullHandling.JsonSchema ? JsonObjectType.Object | JsonObjectType.Null : JsonObjectType.Object,
                        AllowAdditionalProperties = false
                    };
                }

                if (!_schemaResolver.HasSchema(type, false))
                    _schemaGenerator.Generate(type, _schemaResolver);

                if (mayBeNull)
                {
                    if (_settings.NullHandling == NullHandling.JsonSchema)
                    {
                        var schema = new JsonSchema4();
                        schema.OneOf.Add(new JsonSchema4 { Type = JsonObjectType.Null });
                        schema.OneOf.Add(new JsonSchema4 { SchemaReference = _schemaResolver.GetSchema(type, false) });
                        return schema;
                    }
                    else
                    {
                        // TODO: Fix this bad design
                        // IsNullable must be directly set on SwaggerParameter or SwaggerResponse
                        return new JsonSchema4 { SchemaReference = _schemaResolver.GetSchema(type, false) };
                    }
                }
                else
                    return new JsonSchema4 { SchemaReference = _schemaResolver.GetSchema(type, false) };
            }

            if (typeDescription.Type.HasFlag(JsonObjectType.Array))
            {
                var itemType = type.GetEnumerableItemType();
                return new JsonSchema4
                {
                    // TODO: Fix this bad design
                    // IsNullable must be directly set on SwaggerParameter or SwaggerResponse
                    Type = _settings.NullHandling == NullHandling.JsonSchema ? JsonObjectType.Array | JsonObjectType.Null : JsonObjectType.Array,
                    Item = GenerateAndAppendSchemaFromType(itemType, false, null)
                };
            }

            return _schemaGenerator.Generate(type, _schemaResolver);
        }

        private bool IsParameterRequired(ParameterInfo parameter)
        {
            if (parameter == null)
                return false;

            if (parameter.GetCustomAttributes().Any(a => a.GetType().Name == "RequiredAttribute"))
                return true;

            if (parameter.HasDefaultValue)
                return false;

            var isNullable = Nullable.GetUnderlyingType(parameter.ParameterType) != null;
            if (isNullable)
                return false;

            return parameter.ParameterType.GetTypeInfo().IsValueType;
        }

        private bool IsFileResponse(Type returnType)
        {
            return returnType.Name == "IActionResult" ||
                   returnType.Name == "IHttpActionResult" ||
                   returnType.Name == "HttpResponseMessage" ||
                   returnType.InheritsFrom("ActionResult", TypeNameStyle.Name) ||
                   returnType.InheritsFrom("HttpResponseMessage", TypeNameStyle.Name);
        }
    }
}
