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
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Infrastructure;

namespace NSwag.SwaggerGeneration
{
    /// <summary>Provides services to for Swagger generators like the creation of parameters and handling of schemas.</summary>
    public class SwaggerGenerator
    {
        private readonly JsonSchemaGenerator _schemaGenerator;
        private readonly JsonSchemaResolver _schemaResolver;
        private readonly JsonSchemaGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="SwaggerGenerator"/> class.</summary>
        /// <param name="schemaGenerator">The schema generator.</param>
        /// <param name="schemaGeneratorSettings">The schema generator settings.</param>
        /// <param name="schemaResolver">The schema resolver.</param>
        public SwaggerGenerator(JsonSchemaGenerator schemaGenerator, JsonSchemaGeneratorSettings schemaGeneratorSettings, JsonSchemaResolver schemaResolver)
        {
            _schemaGenerator = schemaGenerator;
            _schemaResolver = schemaResolver;
            _settings = schemaGeneratorSettings;
        }

        /// <summary>Creates a path parameter for a given type.</summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <returns>The parameter.</returns>
        public SwaggerParameter CreateUntypedPathParameter(string parameterName, string parameterType)
        {
            var parameter = new SwaggerParameter();
            parameter.Name = parameterName;
            parameter.Kind = SwaggerParameterKind.Path;
            parameter.IsRequired = true;

            if (_settings.SchemaType == SchemaType.Swagger2)
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
        /// <param name="parameter">The parameter information.</param>
        /// <returns>The parameter.</returns>
        public async Task<SwaggerParameter> CreatePrimitiveParameterAsync(string name, ParameterInfo parameter)
        {
            var attributes = parameter.GetCustomAttributes();
            var documentation = await parameter.GetDescriptionAsync(attributes).ConfigureAwait(false);
            return await CreatePrimitiveParameterAsync(name, documentation, parameter.ParameterType, attributes)
                .ConfigureAwait(false);
        }

        /// <summary>Creates a primitive parameter for the given parameter information reflection object.</summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="parentAttributes">The parent attributes.</param>
        /// <returns>The parameter.</returns>
        public async Task<SwaggerParameter> CreatePrimitiveParameterAsync(
            string name, string description, Type parameterType, IEnumerable<Attribute> parentAttributes)
        {
            SwaggerParameter operationParameter;

            var typeDescription = _settings.ReflectionService.GetDescription(parameterType, parentAttributes, _settings);
            if (typeDescription.RequiresSchemaReference(_settings.TypeMappers))
            {
                var schema = await _schemaGenerator
                    .GenerateAsync(parameterType, parentAttributes, _schemaResolver)
                    .ConfigureAwait(false);

                operationParameter = new SwaggerParameter();
                operationParameter.Type = typeDescription.Type;

                if (_settings.SchemaType == SchemaType.Swagger2)
                {
                    operationParameter.CustomSchema = new JsonSchema4 { SchemaReference = schema.ActualSchema };

                    // Copy enumeration for compatibility with other tools which do not understand x-schema.
                    // The enumeration will be ignored by NSwag and only the x-schema is processed
                    if (schema.ActualSchema.IsEnumeration)
                    {
                        operationParameter.Enumeration.Clear();
                        foreach (var item in schema.ActualSchema.Enumeration)
                            operationParameter.Enumeration.Add(item);
                    }
                }
                else
                {
                    // TODO(OpenApi3): How to handle this in OpenApi3?
                    operationParameter.Schema = new JsonSchema4 { SchemaReference = schema.ActualSchema };
                }
            }
            else
            {
                operationParameter = await _schemaGenerator
                    .GenerateAsync<SwaggerParameter>(parameterType, parentAttributes, _schemaResolver)
                    .ConfigureAwait(false);

                if (typeDescription.Type.HasFlag(JsonObjectType.Array))
                    operationParameter.CollectionFormat = SwaggerParameterCollectionFormat.Multi;
            }

            operationParameter.Name = name;
            operationParameter.IsRequired = parentAttributes.TryGetIfAssignableTo("RequiredAttribute", TypeNameStyle.Name) != null;

            if (_settings.SchemaType == SchemaType.Swagger2)
                operationParameter.IsNullableRaw = typeDescription.IsNullable;
            else if (typeDescription.IsNullable)
                operationParameter.Type = typeDescription.Type | JsonObjectType.Null;

            _schemaGenerator.ApplyDataAnnotations(operationParameter, typeDescription, parentAttributes);

            if (description != string.Empty)
                operationParameter.Description = description;

            return operationParameter;
        }

        /// <summary>Creates a primitive parameter for the given parameter information reflection object.</summary>
        /// <param name="name">The name.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The parameter.</returns>
        public async Task<SwaggerParameter> CreateBodyParameterAsync(string name, ParameterInfo parameter)
        {
            var attributes = parameter.GetCustomAttributes();

            var isRequired = IsParameterRequired(parameter);
            var typeDescription = _settings.ReflectionService.GetDescription(parameter.ParameterType, attributes, _settings);

            var operationParameter = new SwaggerParameter
            {
                Name = name,
                Kind = SwaggerParameterKind.Body,
                IsRequired = isRequired,
                IsNullableRaw = typeDescription.IsNullable,
                Description = await parameter.GetDescriptionAsync(attributes).ConfigureAwait(false),
                Schema = await _schemaGenerator.GenerateWithReferenceAndNullability<JsonSchema4>(
                    parameter.ParameterType, attributes, !isRequired, _schemaResolver).ConfigureAwait(false)
            };

            return operationParameter;
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
    }
}
