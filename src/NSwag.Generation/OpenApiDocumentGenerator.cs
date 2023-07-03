//-----------------------------------------------------------------------
// <copyright file="WebApiToSwaggerGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Namotion.Reflection;
using Newtonsoft.Json;
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
            var documentation = contextualParameter.GetDescription(_settings);
            return CreatePrimitiveParameter(name, documentation, contextualParameter);
        }

        /// <summary>Creates a primitive parameter for the given parameter information reflection object.</summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="contextualParameter">Type of the parameter.</param>
        /// <param name="enforceNotNull">Specifies whether the parameter must never be nullable.</param>
        /// <returns>The parameter.</returns>
        public OpenApiParameter CreatePrimitiveParameter(string name, string description, ContextualType contextualParameter, bool enforceNotNull = false)
        {
            var typeDescription = _settings.ReflectionService.GetDescription(contextualParameter, _settings);
            typeDescription.IsNullable = enforceNotNull == false && typeDescription.IsNullable;

            var operationParameter = _settings.SchemaType == SchemaType.Swagger2
                ? CreatePrimitiveSwaggerParameter(contextualParameter, typeDescription)
                : CreatePrimitiveOpenApiParameter(contextualParameter, typeDescription);

            operationParameter.Name = name;
            operationParameter.IsRequired = contextualParameter.ContextAttributes.FirstAssignableToTypeNameOrDefault("RequiredAttribute", TypeNameStyle.Name) != null;
            operationParameter.IsNullableRaw = typeDescription.IsNullable;

            if (description != string.Empty)
            {
                operationParameter.Description = description;
            }

            return operationParameter;
        }

        private OpenApiParameter CreatePrimitiveOpenApiParameter(ContextualType contextualParameter, JsonTypeDescription typeDescription)
        {
            OpenApiParameter operationParameter;
            if (typeDescription.RequiresSchemaReference(_settings.TypeMappers))
            {
                operationParameter = new OpenApiParameter();
                operationParameter.Schema = new JsonSchema();

                _settings.SchemaGenerator.ApplyDataAnnotations(operationParameter.Schema, typeDescription);

                var referencedSchema = _settings.SchemaGenerator.Generate(contextualParameter, _schemaResolver);

                var hasSchemaAnnotations = JsonConvert.SerializeObject(operationParameter.Schema) != "{}";
                if (hasSchemaAnnotations || typeDescription.IsNullable)
                {
                    operationParameter.Schema.IsNullableRaw = true;

                    if (_settings.AllowReferencesWithProperties)
                    {
                        operationParameter.Schema.Reference = referencedSchema.ActualSchema;
                    }
                    else
                    {
                        operationParameter.Schema.OneOf.Add(new JsonSchema { Reference = referencedSchema.ActualSchema });
                    }
                }
                else
                {
                    operationParameter.Schema = new JsonSchema { Reference = referencedSchema.ActualSchema };
                }
            }
            else
            {
                operationParameter = new OpenApiParameter();
                operationParameter.Schema = _settings.SchemaGenerator.GenerateWithReferenceAndNullability<JsonSchema>(
                    contextualParameter, typeDescription.IsNullable, _schemaResolver);

                _settings.SchemaGenerator.ApplyDataAnnotations(operationParameter.Schema, typeDescription);
            }

            if (typeDescription.Type.HasFlag(JsonObjectType.Array))
            {
                operationParameter.Style = OpenApiParameterStyle.Form;
                operationParameter.Explode = true;
            }

            return operationParameter;
        }

        private OpenApiParameter CreatePrimitiveSwaggerParameter(ContextualType contextualParameter, JsonTypeDescription typeDescription)
        {
            OpenApiParameter operationParameter;
            if (typeDescription.RequiresSchemaReference(_settings.TypeMappers))
            {
                var referencedSchema = _settings.SchemaGenerator.Generate(contextualParameter, _schemaResolver);

                operationParameter = new OpenApiParameter
                {
                    Type = typeDescription.Type,
                    CustomSchema = new JsonSchema { Reference = referencedSchema.ActualSchema }
                };

                // Copy enumeration for compatibility with other tools which do not understand x-schema.
                // The enumeration will be ignored by NSwag and only the x-schema is processed
                if (referencedSchema.ActualSchema.IsEnumeration)
                {
                    operationParameter.Enumeration.Clear();
                    foreach (var item in referencedSchema.ActualSchema.Enumeration)
                    {
                        operationParameter.Enumeration.Add(item);
                    }
                }

                _settings.SchemaGenerator.ApplyDataAnnotations(operationParameter, typeDescription);
            }
            else
            {
                operationParameter = _settings.SchemaGenerator.Generate<OpenApiParameter>(contextualParameter, _schemaResolver);
                _settings.SchemaGenerator.ApplyDataAnnotations(operationParameter, typeDescription);
            }

            if (typeDescription.Type.HasFlag(JsonObjectType.Array))
            {
                operationParameter.CollectionFormat = OpenApiParameterCollectionFormat.Multi;
            }

            return operationParameter;
        }
    }
}
