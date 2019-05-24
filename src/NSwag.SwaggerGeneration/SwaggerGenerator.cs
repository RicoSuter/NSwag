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
using Namotion.Reflection;
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
            {
                parameter.IsNullableRaw = false;
            }

            if (parameterType == "guid")
            {
                parameter.Type = JsonObjectType.String;
                parameter.Format = JsonFormatStrings.Guid;
            }
            else if (parameterType == "int" || parameterType == "integer" || parameterType == "short" || parameterType == "long")
            {
                parameter.Type = JsonObjectType.Integer;
            }
            else if (parameterType == "number" || parameterType == "decimal" || parameterType == "double")
            {
                parameter.Type = JsonObjectType.Number;
            }
            else
            {
                parameter.Type = JsonObjectType.String;
            }

            return parameter;
        }

        /// <summary>Creates a primitive parameter for the given parameter information reflection object.</summary>
        /// <param name="name">The name.</param>
        /// <param name="contextualParameter">The parameter.</param>
        /// <returns>The parameter.</returns>
        public async Task<SwaggerParameter> CreatePrimitiveParameterAsync(string name, ContextualParameterInfo contextualParameter)
        {
            var documentation = await contextualParameter.GetDescriptionAsync().ConfigureAwait(false);
            return await CreatePrimitiveParameterAsync(name, documentation, contextualParameter).ConfigureAwait(false);
        }

        /// <summary>Creates a primitive parameter for the given parameter information reflection object.</summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="contextualParameter">Type of the parameter.</param>
        /// <returns>The parameter.</returns>
        public async Task<SwaggerParameter> CreatePrimitiveParameterAsync(
            string name, string description, ContextualType contextualParameter)
        {
            SwaggerParameter operationParameter;

            var typeDescription = _settings.ReflectionService.GetDescription(contextualParameter, _settings);
            if (typeDescription.RequiresSchemaReference(_settings.TypeMappers))
            {
                var schema = await _schemaGenerator
                    .GenerateAsync(contextualParameter, _schemaResolver)
                    .ConfigureAwait(false);

                operationParameter = new SwaggerParameter();

                if (_settings.SchemaType == SchemaType.Swagger2)
                {
                    operationParameter.Type = typeDescription.Type;
                    operationParameter.CustomSchema = new JsonSchema { Reference = schema.ActualSchema };

                    // Copy enumeration for compatibility with other tools which do not understand x-schema.
                    // The enumeration will be ignored by NSwag and only the x-schema is processed
                    if (schema.ActualSchema.IsEnumeration)
                    {
                        operationParameter.Enumeration.Clear();
                        foreach (var item in schema.ActualSchema.Enumeration)
                        {
                            operationParameter.Enumeration.Add(item);
                        }
                    }
                }
                else
                {
                    if (typeDescription.IsNullable)
                    {
                        operationParameter.Schema = new JsonSchema { IsNullableRaw = true };
                        operationParameter.Schema.OneOf.Add(new JsonSchema { Reference = schema.ActualSchema });
                    }
                    else
                    {
                        operationParameter.Schema = new JsonSchema { Reference = schema.ActualSchema };
                    }
                }
            }
            else
            {
                if (_settings.SchemaType == SchemaType.Swagger2)
                {
                    operationParameter = await _schemaGenerator
                        .GenerateAsync<SwaggerParameter>(contextualParameter, _schemaResolver)
                        .ConfigureAwait(false);
                }
                else
                {
                    operationParameter = new SwaggerParameter
                    {
                        Schema = await _schemaGenerator
                            .GenerateWithReferenceAndNullabilityAsync<JsonSchema>(
                                contextualParameter, typeDescription.IsNullable, _schemaResolver)
                            .ConfigureAwait(false)
                    };
                }
            }

            operationParameter.Name = name;
            operationParameter.IsRequired = contextualParameter.ContextAttributes.FirstAssignableToTypeNameOrDefault("RequiredAttribute", TypeNameStyle.Name) != null;

            if (typeDescription.Type.HasFlag(JsonObjectType.Array))
            {
                operationParameter.CollectionFormat = SwaggerParameterCollectionFormat.Multi;
            }

            operationParameter.IsNullableRaw = typeDescription.IsNullable;
            _schemaGenerator.ApplyDataAnnotations(operationParameter, typeDescription, contextualParameter.ContextAttributes);

            if (description != string.Empty)
            {
                operationParameter.Description = description;
            }

            return operationParameter;
        }

        private bool IsParameterRequired(ParameterInfo parameter)
        {
            if (parameter == null)
            {
                return false;
            }

            if (parameter.GetCustomAttributes().Any(a => a.GetType().Name == "RequiredAttribute"))
            {
                return true;
            }

            if (parameter.HasDefaultValue)
            {
                return false;
            }

            var isNullable = Nullable.GetUnderlyingType(parameter.ParameterType) != null;
            if (isNullable)
            {
                return false;
            }

            return parameter.ParameterType.GetTypeInfo().IsValueType;
        }
    }
}
