﻿//-----------------------------------------------------------------------
// <copyright file="WebApiToSwaggerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Annotations;
using NJsonSchema.Generation;
using NJsonSchema.Infrastructure;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace NSwag.Generation.AspNetCore.Processors
{
    internal sealed class OperationParameterProcessor : IOperationProcessor
    {
        private static readonly Regex unusedPathParametersRegex = new("{(.*?)(:(([^/]*)?))?}", RegexOptions.Compiled);

        private const string MultipartFormData = "multipart/form-data";

        private readonly AspNetCoreOpenApiDocumentGeneratorSettings _settings;

        public OperationParameterProcessor(AspNetCoreOpenApiDocumentGeneratorSettings settings)
        {
            _settings = settings;
        }

        /// <summary>Processes the specified method information.</summary>
        /// <param name="operationProcessorContext"></param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public bool Process(OperationProcessorContext operationProcessorContext)
        {
            if (operationProcessorContext is not AspNetCoreOperationProcessorContext context)
            {
                return false;
            }

            var httpPath = context.OperationDescription.Path;
            var parameters = context.ApiDescription.ParameterDescriptions;
            var methodParameters = context.MethodInfo?.GetParameters() ?? [];

            var position = operationProcessorContext.Parameters.Count(x => x.Value.Kind is not OpenApiParameterKind.Body) + 1;
            foreach (var apiParameter in parameters.Where(p =>
                p.Source != null &&
                (p.ModelMetadata == null || p.ModelMetadata.IsBindingAllowed)))
            {
                // TODO: Provide extension point so that this can be implemented in the ApiVersionProcessor class
                var versionProcessor = _settings.OperationProcessors.TryGet<ApiVersionProcessor>();
                if (versionProcessor != null &&
                    versionProcessor.IgnoreParameter &&
                    apiParameter.ModelMetadata?.DataTypeName == "ApiVersion")
                {
                    continue;
                }

                // In Mvc < 2.0, there isn't a good way to infer the attributes of a parameter with a IModelNameProvider.Name
                // value that's different than the parameter name. Additionally, ApiExplorer will recurse in to complex model bound types
                // and expose properties as top level parameters. Consequently, determining the property or parameter of an Api is best
                // effort attempt.
                var extendedApiParameter = new ExtendedApiParameterDescription(_settings.SchemaSettings)
                {
                    ApiParameter = apiParameter,
                    Attributes = [],
                    ParameterType = apiParameter.Type
                };

                ParameterInfo parameter = null;

                var propertyName = apiParameter.ModelMetadata?.PropertyName;
                var property = !string.IsNullOrEmpty(propertyName) ?
                    apiParameter.ModelMetadata.ContainerType?.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance) :
                    null;

                if (property != null)
                {
                    extendedApiParameter.PropertyInfo = property;
                    extendedApiParameter.Attributes = Attribute.GetCustomAttributes(property);
                }
                else
                {
                    var parameterDescriptor = apiParameter.TryGetPropertyValue<ParameterDescriptor>("ParameterDescriptor");
                    var parameterName = parameterDescriptor?.Name ?? apiParameter.Name;
                    parameter = methodParameters.FirstOrDefault(m => m.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
                    if (parameter != null)
                    {
                        extendedApiParameter.ParameterInfo = parameter;
                        extendedApiParameter.Attributes = Attribute.GetCustomAttributes(parameter);
                    }
                    else if (operationProcessorContext.ControllerType != null)
                    {
                        parameterName = apiParameter.Name;
                        property = operationProcessorContext.ControllerType.GetProperty(parameterName, BindingFlags.Public | BindingFlags.Instance);
                        if (property != null)
                        {
                            extendedApiParameter.PropertyInfo = property;
                            extendedApiParameter.Attributes = Attribute.GetCustomAttributes(property);
                        }
                    }
                }

                if (apiParameter.Type == null)
                {
                    extendedApiParameter.ParameterType = typeof(string);

                    if (apiParameter.Source == BindingSource.Path)
                    {
                        // ignore unused implicit path parameters
                        if (!httpPath.Contains("{" + apiParameter.Name + ":", StringComparison.OrdinalIgnoreCase) &&
                            !httpPath.Contains("{" + apiParameter.Name + "}", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        extendedApiParameter.Attributes = [.. extendedApiParameter.Attributes, new NotNullAttribute()];
                    }
                }

                if (extendedApiParameter.Attributes.GetAssignableToTypeName("SwaggerIgnoreAttribute", TypeNameStyle.Name).Any())
                {
                    continue;
                }

                OpenApiParameter operationParameter = null;
                if (apiParameter.Source == BindingSource.Path ||
                    (apiParameter.Source == BindingSource.Custom &&
                     httpPath.Contains($"{{{apiParameter.Name}}}")))
                {
                    // required path parameters are not nullable
                    var enforceNotNull = apiParameter.RouteInfo?.IsOptional == false;

                    operationParameter = CreatePrimitiveParameter(context, extendedApiParameter, enforceNotNull);
                    operationParameter.Kind = OpenApiParameterKind.Path;
                    operationParameter.IsRequired = true; // apiParameter.RouteInfo?.IsOptional == false;

                    context.OperationDescription.Operation.Parameters.Add(operationParameter);
                }
                else if (apiParameter.Source == BindingSource.Header)
                {
                    operationParameter = CreatePrimitiveParameter(context, extendedApiParameter);
                    operationParameter.Kind = OpenApiParameterKind.Header;

                    context.OperationDescription.Operation.Parameters.Add(operationParameter);
                }
                else if (apiParameter.Source == BindingSource.Query)
                {
                    operationParameter = CreatePrimitiveParameter(context, extendedApiParameter);
                    operationParameter.Kind = OpenApiParameterKind.Query;

                    context.OperationDescription.Operation.Parameters.Add(operationParameter);
                }
                else if (apiParameter.Source == BindingSource.Body)
                {
                    operationParameter = AddBodyParameter(context, extendedApiParameter);
                }
                else if (apiParameter.Source == BindingSource.Form)
                {
                    if (_settings.SchemaSettings.SchemaType == SchemaType.Swagger2)
                    {
                        operationParameter = CreatePrimitiveParameter(context, extendedApiParameter);
                        operationParameter.Kind = OpenApiParameterKind.FormData;
                        context.OperationDescription.Operation.Parameters.Add(operationParameter);
                    }
                    else
                    {
                        var schema = CreateOrGetFormDataSchema(context);
                        schema.Properties[extendedApiParameter.ApiParameter.Name] = CreateFormDataProperty(context, extendedApiParameter, schema);
                    }
                }
                else
                {
                    if (!TryAddFileParameter(context, extendedApiParameter))
                    {
                        operationParameter = CreatePrimitiveParameter(context, extendedApiParameter);
                        operationParameter.Kind = OpenApiParameterKind.Query;

                        context.OperationDescription.Operation.Parameters.Add(operationParameter);
                    }
                }

                if (operationParameter != null)
                {
                    operationParameter.Position = position;
                    position++;

                    if (_settings.SchemaSettings.SchemaType == SchemaType.OpenApi3)
                    {
                        operationParameter.IsNullableRaw = null;
                    }

                    if (parameter != null)
                    {
                        if (_settings.GenerateOriginalParameterNames && operationParameter.Name != parameter.Name)
                        {
                            operationParameter.OriginalName = parameter.Name;
                        }

                        ((Dictionary<ParameterInfo, OpenApiParameter>)operationProcessorContext.Parameters)[parameter] = operationParameter;
                    }
                }
            }

            ApplyOpenApiBodyParameterAttribute(context.OperationDescription, context.MethodInfo);

            var actualParameters = context.OperationDescription.Operation.GetActualParameters().ToList();
            RemoveUnusedPathParameters(context.OperationDescription, actualParameters, httpPath);
            UpdateConsumedTypes(context.OperationDescription, actualParameters);
            EnsureSingleBodyParameter(context.OperationDescription, actualParameters);

            return true;
        }

        private void ApplyOpenApiBodyParameterAttribute(OpenApiOperationDescription operationDescription, MethodInfo methodInfo)
        {
            dynamic bodyParameterAttribute = methodInfo?
                .GetCustomAttributes()
                .FirstAssignableToTypeNameOrDefault("OpenApiBodyParameterAttribute", TypeNameStyle.Name);

            if (bodyParameterAttribute != null)
            {
                if (operationDescription.Operation.RequestBody == null)
                {
                    operationDescription.Operation.RequestBody = new OpenApiRequestBody();
                }

                var mimeTypes = ObjectExtensions.HasProperty(bodyParameterAttribute, "MimeType") ?
                    new string[] { bodyParameterAttribute.MimeType } : bodyParameterAttribute.MimeTypes;

                foreach (var mimeType in mimeTypes)
                {
                    operationDescription.Operation.RequestBody.Content[mimeType] = new OpenApiMediaType
                    {
                        Schema = mimeType == "application/json" ? JsonSchema.CreateAnySchema() : new JsonSchema
                        {
                            Type = _settings.SchemaSettings.SchemaType == SchemaType.Swagger2 ? JsonObjectType.File : JsonObjectType.String,
                            Format = _settings.SchemaSettings.SchemaType == SchemaType.Swagger2 ? null : JsonFormatStrings.Binary,
                        }
                    };
                }
            }
        }

        private static void EnsureSingleBodyParameter(OpenApiOperationDescription operationDescription, List<OpenApiParameter> actualParameters)
        {
            if (actualParameters.Count(p => p.Kind == OpenApiParameterKind.Body) > 1)
            {
                throw new InvalidOperationException($"The operation '{operationDescription.Operation.OperationId}' has more than one body parameter.");
            }
        }

        private static void UpdateConsumedTypes(OpenApiOperationDescription operationDescription, List<OpenApiParameter> actualParameters)
        {
            if (actualParameters.Any(static p => p.IsBinary || p.ActualSchema.IsBinary))
            {
                operationDescription.Operation.TryAddConsumes("multipart/form-data");
            }
        }

        private static void RemoveUnusedPathParameters(OpenApiOperationDescription operationDescription, List<OpenApiParameter> actualParameters, string httpPath)
        {
            operationDescription.Path = "/" + unusedPathParametersRegex.Replace(httpPath, match =>
            {
                var parameterName = match.Groups[1].Value.TrimEnd('?');
                if (actualParameters.Any(p => p.Kind == OpenApiParameterKind.Path && string.Equals(p.Name, parameterName, StringComparison.OrdinalIgnoreCase)))
                {
                    return "{" + parameterName + "}";
                }

                return string.Empty;
            }).Trim('/');
        }

        private bool TryAddFileParameter(
            OperationProcessorContext context, ExtendedApiParameterDescription extendedApiParameter)
        {
            var typeInfo = _settings.SchemaSettings.ReflectionService.GetDescription(extendedApiParameter.ParameterType.ToContextualType(extendedApiParameter.Attributes), _settings.SchemaSettings);

            var isFileArray = IsFileArray(extendedApiParameter.ApiParameter.Type, typeInfo);

            var attributes = extendedApiParameter.Attributes
                .Union(extendedApiParameter.ParameterType.GetTypeInfo().GetCustomAttributes());

            var hasSwaggerFileAttribute = attributes.FirstAssignableToTypeNameOrDefault("SwaggerFileAttribute", TypeNameStyle.Name) != null;

            if (typeInfo.Type == JsonObjectType.File ||
                typeInfo.Format == JsonFormatStrings.Binary ||
                hasSwaggerFileAttribute ||
                isFileArray)
            {
                AddFileParameter(context, extendedApiParameter, isFileArray);
                return true;
            }

            return false;
        }

        private void AddFileParameter(OperationProcessorContext context, ExtendedApiParameterDescription extendedApiParameter, bool isFileArray)
        {
            if (_settings.SchemaSettings.SchemaType == SchemaType.Swagger2)
            {
                var operationParameter = CreatePrimitiveParameter(context, extendedApiParameter);
                operationParameter.Type = JsonObjectType.File;
                operationParameter.Kind = OpenApiParameterKind.FormData;

                if (isFileArray)
                {
                    operationParameter.CollectionFormat = OpenApiParameterCollectionFormat.Multi;
                }

                context.OperationDescription.Operation.Parameters.Add(operationParameter);
            }
            else
            {
                var schema = CreateOrGetFormDataSchema(context);
                schema.Type = JsonObjectType.Object;
                schema.Properties[extendedApiParameter.ApiParameter.Name] = CreateFormDataProperty(context, extendedApiParameter, schema);
            }
        }

        private static JsonSchema CreateOrGetFormDataSchema(OperationProcessorContext context)
        {
            if (context.OperationDescription.Operation.RequestBody == null)
            {
                context.OperationDescription.Operation.RequestBody = new OpenApiRequestBody();
            }

            var requestBody = context.OperationDescription.Operation.RequestBody;
            if (!requestBody.Content.TryGetValue(MultipartFormData, out OpenApiMediaType value))
            {
                value = new OpenApiMediaType
                {
                    Schema = new JsonSchema()
                };
                requestBody.Content[MultipartFormData] = value;
            }

            return value.Schema ??= new JsonSchema();
        }

        private static JsonSchemaProperty CreateFormDataProperty(OperationProcessorContext context, ExtendedApiParameterDescription extendedApiParameter, JsonSchema schema)
        {
            return context.SchemaGenerator.GenerateWithReferenceAndNullability<JsonSchemaProperty>(
               extendedApiParameter.ApiParameter.Type.ToContextualType(extendedApiParameter.Attributes), context.SchemaResolver);
        }

        private bool IsFileArray(Type type, JsonTypeDescription typeInfo)
        {
            var isFormFileCollection = type.Name == "IFormFileCollection";
            if (isFormFileCollection)
            {
                return true;
            }

            if (typeInfo.Type == JsonObjectType.Array && type.GenericTypeArguments.Length > 0)
            {
                var description = _settings.SchemaSettings.ReflectionService.GetDescription(type.GenericTypeArguments[0].ToContextualType(), _settings.SchemaSettings);
                if (description.Type == JsonObjectType.File || description.Format == JsonFormatStrings.Binary)
                {
                    return true;
                }
            }

            return false;
        }

        private OpenApiParameter AddBodyParameter(OperationProcessorContext context, ExtendedApiParameterDescription extendedApiParameter)
        {
            OpenApiParameter operationParameter;

            var contextualParameterType = extendedApiParameter.ParameterType
                .ToContextualType(extendedApiParameter.Attributes);

            var typeDescription = _settings.SchemaSettings.ReflectionService.GetDescription(contextualParameterType, _settings.SchemaSettings);
            var isNullable = _settings.AllowNullableBodyParameters && typeDescription.IsNullable;
            var operation = context.OperationDescription.Operation;

            var parameterType = extendedApiParameter.ParameterType;
            if (parameterType.Name == "XmlDocument" || parameterType.InheritsFromTypeName("XmlDocument", TypeNameStyle.Name))
            {
                operation.TryAddConsumes("application/xml");
                operationParameter = new OpenApiParameter
                {
                    Name = extendedApiParameter.ApiParameter.Name,
                    Kind = OpenApiParameterKind.Body,
                    Schema = new JsonSchema
                    {
                        Type = JsonObjectType.String,
                        IsNullableRaw = isNullable
                    },
                    IsNullableRaw = isNullable,
                    IsRequired = extendedApiParameter.IsRequired(_settings.RequireParametersWithoutDefault),
                    Description = extendedApiParameter.GetDocumentation()
                };
            }
            else if (parameterType.IsAssignableToTypeName("System.IO.Stream", TypeNameStyle.FullName))
            {
                operation.TryAddConsumes("application/octet-stream");
                operationParameter = new OpenApiParameter
                {
                    Name = extendedApiParameter.ApiParameter.Name,
                    Kind = OpenApiParameterKind.Body,
                    Schema = new JsonSchema
                    {
                        Type = JsonObjectType.String,
                        Format = JsonFormatStrings.Binary,
                        IsNullableRaw = isNullable
                    },
                    IsNullableRaw = isNullable,
                    IsRequired = extendedApiParameter.IsRequired(_settings.RequireParametersWithoutDefault),
                    Description = extendedApiParameter.GetDocumentation()
                };
            }
            else // body from type
            {
                operationParameter = new OpenApiParameter
                {
                    Name = extendedApiParameter.ApiParameter.Name,
                    Kind = OpenApiParameterKind.Body,
                    IsRequired = extendedApiParameter.IsRequired(_settings.RequireParametersWithoutDefault),
                    IsNullableRaw = isNullable,
                    Description = extendedApiParameter.GetDocumentation(),
                    Schema = context.SchemaGenerator.GenerateWithReferenceAndNullability<JsonSchema>(
                        contextualParameterType, isNullable, schemaResolver: context.SchemaResolver)
                };
            }

            operation.Parameters.Add(operationParameter);
            return operationParameter;
        }

        private OpenApiParameter CreatePrimitiveParameter(
            OperationProcessorContext context,
            ExtendedApiParameterDescription extendedApiParameter,
            bool enforceNotNull = false)
        {
            var contextualParameterType =
                extendedApiParameter.ParameterInfo?.ToContextualParameter()?.ParameterType ??
                extendedApiParameter.PropertyInfo?.ToContextualProperty()?.PropertyType ??
                extendedApiParameter.ParameterType.ToContextualType(extendedApiParameter.Attributes);

            var description = extendedApiParameter.GetDocumentation();
            var operationParameter = context.DocumentGenerator.CreatePrimitiveParameter(
                extendedApiParameter.ApiParameter.Name, description, contextualParameterType, enforceNotNull);

            var exampleValue = extendedApiParameter.PropertyInfo != null ?
                context.SchemaGenerator.GenerateExample(extendedApiParameter.PropertyInfo.ToContextualAccessor()) : null;

            var hasExampleValue = exampleValue != null;
            var hasDefaultValue = extendedApiParameter.ParameterInfo?.HasDefaultValue == true;

            if (hasExampleValue || hasDefaultValue)
            {
                var defaultValue = hasDefaultValue ? context.SchemaGenerator
                    .ConvertDefaultValue(contextualParameterType, extendedApiParameter.ParameterInfo.DefaultValue) : null;

                if (_settings.SchemaSettings.SchemaType == SchemaType.Swagger2)
                {
                    operationParameter.Default = defaultValue;
                    operationParameter.Example = exampleValue;
                }
                else if (operationParameter.Schema.HasReference)
                {
                    if (_settings.SchemaSettings.AllowReferencesWithProperties)
                    {
                        operationParameter.Schema = new JsonSchema
                        {
                            Default = defaultValue,
                            Example = exampleValue,
                            Reference = operationParameter.Schema,
                        };
                    }
                    else
                    {
                        operationParameter.Schema = new JsonSchema
                        {
                            Default = defaultValue,
                            Example = exampleValue,
                            OneOf = { operationParameter.Schema },
                        };
                    }
                }
                else
                {
                    operationParameter.Schema.Default = defaultValue;
                    operationParameter.Schema.Example = exampleValue;
                }
            }

            operationParameter.IsRequired = extendedApiParameter.IsRequired(_settings.RequireParametersWithoutDefault);
            return operationParameter;
        }

        private sealed class ExtendedApiParameterDescription
        {
            private readonly IXmlDocsSettings _xmlDocsSettings;

            public ApiParameterDescription ApiParameter { get; set; }

            public ParameterInfo ParameterInfo { get; set; }

            public PropertyInfo PropertyInfo { get; set; }

            public Type ParameterType { get; set; }

            public Attribute[] Attributes { get; set; } = [];

            public ExtendedApiParameterDescription(IXmlDocsSettings xmlDocsSettings)
            {
                _xmlDocsSettings = xmlDocsSettings;
            }

            public bool IsRequired(bool requireParametersWithoutDefault)
            {
                var isRequired = false;

                // available in asp.net core >= 2.2
                if (ApiParameter.HasProperty("IsRequired"))
                {
                    isRequired = ApiParameter.TryGetPropertyValue("IsRequired", false);
                }
                else
                {
                    // fallback for asp.net core <= 2.1
                    if (ApiParameter.Source == BindingSource.Body)
                    {
                        isRequired = true;
                    }
                    else if (ApiParameter.ModelMetadata != null && ApiParameter.ModelMetadata.IsBindingRequired)
                    {
                        isRequired = true;
                    }
                    else if (ApiParameter.Source == BindingSource.Path && ApiParameter.RouteInfo != null && !ApiParameter.RouteInfo.IsOptional)
                    {
                        isRequired = true;
                    }
                }

                return isRequired || (requireParametersWithoutDefault && ParameterInfo?.HasDefaultValue != true);
            }

            public string GetDocumentation()
            {
                var parameterDocumentation = string.Empty;
                if (ParameterInfo != null)
                {
                    parameterDocumentation = ParameterInfo.ToContextualParameter().GetDescription(_xmlDocsSettings);
                }
                else if (PropertyInfo != null)
                {
                    parameterDocumentation = PropertyInfo.ToContextualProperty().GetDescription(_xmlDocsSettings);
                }

                return parameterDocumentation;
            }
        }
    }
}
