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
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Infrastructure;

namespace NSwag.SwaggerGeneration
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
        public async Task<SwaggerParameter> CreatePrimitiveParameterAsync(string name, ParameterInfo parameter)
        {
            var documentation = await parameter.GetDescriptionAsync(parameter.GetCustomAttributes()).ConfigureAwait(false);
            return await CreatePrimitiveParameterAsync(name, documentation, parameter.ParameterType, parameter.GetCustomAttributes().ToList()).ConfigureAwait(false);
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
        public async Task<SwaggerParameter> CreatePrimitiveParameterAsync(string name, string description, Type parameterType, IList<Attribute> parentAttributes)
        {
            var typeDescription = JsonObjectTypeDescription.FromType(parameterType, ResolveContract(parameterType), parentAttributes, _settings.DefaultEnumHandling);

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
                        SchemaReference = await _schemaGenerator.GenerateAsync(parameterType, parentAttributes, _schemaResolver).ConfigureAwait(false)
                    }
                };
            }
            else
            {
                var hasTypeMapper = _settings.TypeMappers.Any(tm => tm.MappedType == parameterType);
                if (!hasTypeMapper)
                    parameterType = typeDescription.Type.HasFlag(JsonObjectType.Object) ? typeof(string) : parameterType; // object types must be treated as string

                operationParameter = await _schemaGenerator.GenerateAsync<SwaggerParameter>(parameterType, parentAttributes, _schemaResolver).ConfigureAwait(false);
                _schemaGenerator.ApplyPropertyAnnotations(operationParameter, new Newtonsoft.Json.Serialization.JsonProperty(), parameterType, parentAttributes, typeDescription);

                // check if the type mapper did not properly change the type to a primitive
                if (hasTypeMapper && typeDescription.Type.HasFlag(JsonObjectType.Object) && operationParameter.Type == JsonObjectType.Object)
                    operationParameter.Type = JsonObjectType.String; // enforce string as default
            }

            operationParameter.Name = name;
            operationParameter.IsRequired = parentAttributes?.Any(a => a.GetType().Name == "RequiredAttribute") ?? false;
            operationParameter.IsNullableRaw = typeDescription.IsNullable;

            if (typeDescription.Type.HasFlag(JsonObjectType.Array))
                operationParameter.CollectionFormat = SwaggerParameterCollectionFormat.Multi;

            if (description != string.Empty)
                operationParameter.Description = description;

            return operationParameter;
        }

        /// <summary>Gets the contract for the given type.</summary>
        /// <param name="parameterType"></param>
        /// <returns>The contract.</returns>
        public JsonContract ResolveContract(Type parameterType)
        {
            return _settings.ActualContractResolver.ResolveContract(parameterType);
        }

        /// <summary>Creates a primitive parameter for the given parameter information reflection object.</summary>
        /// <param name="name">The name.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        public async Task<SwaggerParameter> CreateBodyParameterAsync(string name, ParameterInfo parameter)
        {
            var isRequired = IsParameterRequired(parameter);

            var typeDescription = JsonObjectTypeDescription.FromType(parameter.ParameterType, ResolveContract(parameter.ParameterType), parameter.GetCustomAttributes(), _settings.DefaultEnumHandling);
            var operationParameter = new SwaggerParameter
            {
                Name = name,
                Kind = SwaggerParameterKind.Body,
                IsRequired = isRequired,
                IsNullableRaw = typeDescription.IsNullable,
                Schema = await GenerateAndAppendSchemaFromTypeAsync(parameter.ParameterType, !isRequired, parameter.GetCustomAttributes()).ConfigureAwait(false),
            };

            operationParameter.Description = await parameter.GetDescriptionAsync(parameter.GetCustomAttributes()).ConfigureAwait(false);
            return operationParameter;
        }

        /// <summary>Generates and appends a schema from a given type.</summary>
        /// <param name="type">The type.</param>
        /// <param name="mayBeNull">if set to <c>true</c> [may be null].</param>
        /// <param name="parentAttributes">The parent attributes.</param>
        /// <returns>The schema.</returns>
        public async Task<JsonSchema4> GenerateAndAppendSchemaFromTypeAsync(Type type, bool mayBeNull, IEnumerable<Attribute> parentAttributes)
        {
            if (type.Name == "Task`1")
                type = type.GenericTypeArguments[0];

            if (type.Name == "JsonResult`1")
                type = type.GenericTypeArguments[0];

            if (IsFileResponse(type))
                return new JsonSchema4 { Type = JsonObjectType.File };

            // TODO: Merge with NJS.LoadPropertyOrFieldAsync
            var typeDescription = JsonObjectTypeDescription.FromType(type, ResolveContract(type), parentAttributes, _settings.DefaultEnumHandling);
            // TODO: Use needsSchemaReference from NJS here
            if ((typeDescription.Type.HasFlag(JsonObjectType.Object) || typeDescription.IsEnum) && !typeDescription.IsDictionary)
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

                var responseSchema = await _schemaGenerator.GenerateAsync(type, parentAttributes, _schemaResolver).ConfigureAwait(false);
                if (mayBeNull)
                {
                    if (_settings.NullHandling != NullHandling.Swagger)
                    {
                        var schema = new JsonSchema4();
                        schema.OneOf.Add(new JsonSchema4 { Type = JsonObjectType.Null });
                        schema.OneOf.Add(new JsonSchema4 { SchemaReference = responseSchema.ActualSchema });
                        return schema;
                    }
                    else
                    {
                        // TODO: Fix this bad design
                        // IsNullable must be directly set on SwaggerParameter or SwaggerResponse
                        return new JsonSchema4 { SchemaReference = responseSchema.ActualSchema };
                    }
                }
                else
                    return new JsonSchema4 { SchemaReference = responseSchema.ActualSchema };
            }

            if (typeDescription.Type.HasFlag(JsonObjectType.Array))
            {
                var jsonType = _settings.NullHandling == NullHandling.JsonSchema
                    ? JsonObjectType.Array | JsonObjectType.Null
                    : JsonObjectType.Array;

                var itemType = type.GetEnumerableItemType();
                var itemSchema = itemType != null
                    ? await GenerateAndAppendSchemaFromTypeAsync(itemType, false, null).ConfigureAwait(false)
                    : JsonSchema4.CreateAnySchema();

                var schema = new JsonSchema4
                {
                    Type = jsonType,
                    Item = itemSchema
                };

                if (mayBeNull && _settings.NullHandling != NullHandling.Swagger)
                {
                    schema.OneOf.Add(new JsonSchema4 { Type = JsonObjectType.Null });
                    schema.OneOf.Add(new JsonSchema4 { SchemaReference = schema });
                    return schema;
                }

                return schema;

                // TODO: Fix this bad design
                // IsNullable must be directly set on SwaggerParameter or SwaggerResponse
            }

            return await _schemaGenerator.GenerateAsync(type, parentAttributes, _schemaResolver).ConfigureAwait(false);
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

            return returnType.IsAssignableTo("FileResult", TypeNameStyle.Name) ||
                   returnType.Name == "IActionResult" ||
                   returnType.Name == "IHttpActionResult" ||
                   returnType.Name == "HttpResponseMessage" ||
                   returnType.InheritsFrom("ActionResult", TypeNameStyle.Name) ||
                   returnType.InheritsFrom("HttpResponseMessage", TypeNameStyle.Name);
        }
    }
}
