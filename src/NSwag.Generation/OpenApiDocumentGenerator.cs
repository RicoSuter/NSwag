//-----------------------------------------------------------------------
// <copyright file="WebApiToSwaggerGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Infrastructure;

namespace NSwag.Generation
{
    /// <summary>Provides services to for Swagger generators like the creation of parameters and handling of schemas.</summary>
    public class OpenApiDocumentGenerator
    {
        private readonly JsonSchemaResolver _schemaResolver;
        private readonly OpenApiDocumentGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="OpenApiDocumentGenerator"/> class.</summary>
        /// <param name="settings">The settings.</param>
        /// <param name="schemaResolver">The schema resolver.</param>
        public OpenApiDocumentGenerator(OpenApiDocumentGeneratorSettings settings, JsonSchemaResolver schemaResolver)
        {
            _schemaResolver = schemaResolver;
            _settings = settings;
        }

        /// <summary>Creates a path parameter for a given type.</summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <returns>The parameter.</returns>
        public OpenApiParameter CreateUntypedPathParameter(string parameterName, string parameterType)
        {
            var parameter = new OpenApiParameter();
            parameter.Name = parameterName;
            parameter.Kind = OpenApiParameterKind.Path;
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
        public OpenApiParameter CreatePrimitiveParameter(string name, ContextualParameterInfo contextualParameter)
        {
            var documentation = contextualParameter.GetDescription();
            return CreatePrimitiveParameter(name, documentation, contextualParameter);
        }

        /// <summary>Creates a primitive parameter for the given parameter information reflection object.</summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="contextualParameter">Type of the parameter.</param>
        /// <returns>The parameter.</returns>
        public OpenApiParameter CreatePrimitiveParameter(
            string name, string description, ContextualType contextualParameter)
        {
            OpenApiParameter operationParameter;

            var typeDescription = _settings.ReflectionService.GetDescription(contextualParameter, _settings);
            if (typeDescription.RequiresSchemaReference(_settings.TypeMappers))
            {
                var schema = _settings.SchemaGenerator
                    .Generate(contextualParameter, _schemaResolver);

                operationParameter = new OpenApiParameter();

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
                    operationParameter = _settings.SchemaGenerator
                        .Generate<OpenApiParameter>(contextualParameter, _schemaResolver);
                }
                else
                {
                    operationParameter = new OpenApiParameter
                    {
                        Schema = _settings.SchemaGenerator
                            .GenerateWithReferenceAndNullability<JsonSchema>(
                                contextualParameter, typeDescription.IsNullable, _schemaResolver)
                    };
                }
            }

            operationParameter.Name = name;
            operationParameter.IsRequired = contextualParameter.ContextAttributes.FirstAssignableToTypeNameOrDefault("RequiredAttribute", TypeNameStyle.Name) != null;

            if (typeDescription.Type.HasFlag(JsonObjectType.Array))
            {
                operationParameter.CollectionFormat = OpenApiParameterCollectionFormat.Multi;
            }

            operationParameter.IsNullableRaw = typeDescription.IsNullable;
            _settings.SchemaGenerator.ApplyDataAnnotations(operationParameter, contextualParameter, typeDescription);

            if (description != string.Empty)
            {
                operationParameter.Description = description;
            }

            return operationParameter;
        }
    }
}
