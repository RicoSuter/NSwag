//-----------------------------------------------------------------------
// <copyright file="OperationParameterProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Infrastructure;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace NSwag.Generation.WebApi.Processors
{
    /// <summary>Generates the operation's parameters.</summary>
    public class OperationParameterProcessor : IOperationProcessor
    {
        private readonly WebApiOpenApiDocumentGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="OperationParameterProcessor"/> class.</summary>
        /// <param name="settings">The settings.</param>
        public OperationParameterProcessor(WebApiOpenApiDocumentGeneratorSettings settings)
        {
            _settings = settings;
        }

        /// <summary>Processes the specified method information.</summary>
        /// <param name="context"></param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public bool Process(OperationProcessorContext context)
        {
            var httpPath = context.OperationDescription.Path;
            var parameters = context.MethodInfo.GetParameters();

            var position = 1;
            foreach (var contextualParameter in parameters.Select(p => p.ToContextualParameter())
                .Where(p => p.Type != typeof(CancellationToken) &&
                            !p.ContextAttributes.GetAssignableToTypeName("SwaggerIgnoreAttribute", TypeNameStyle.Name).Any() &&
                            !p.ContextAttributes.GetAssignableToTypeName("FromServicesAttribute", TypeNameStyle.Name).Any() &&
                            !p.ContextAttributes.GetAssignableToTypeName("BindNeverAttribute", TypeNameStyle.Name).Any()))
            {
                var parameterName = contextualParameter.Name;

                dynamic fromRouteAttribute = contextualParameter.ContextAttributes.FirstAssignableToTypeNameOrDefault("Microsoft.AspNetCore.Mvc.FromRouteAttribute");
                dynamic fromHeaderAttribute = contextualParameter.ContextAttributes.FirstAssignableToTypeNameOrDefault("Microsoft.AspNetCore.Mvc.FromHeaderAttribute");
                dynamic fromFormAttribute = contextualParameter.ContextAttributes.FirstAssignableToTypeNameOrDefault("Microsoft.AspNetCore.Mvc.FromFormAttribute");

                var fromBodyAttribute = contextualParameter.ContextAttributes.FirstAssignableToTypeNameOrDefault("FromBodyAttribute", TypeNameStyle.Name);
                var fromUriAttribute = contextualParameter.ContextAttributes.FirstAssignableToTypeNameOrDefault("FromUriAttribute", TypeNameStyle.Name) ??
                                       contextualParameter.ContextAttributes.FirstAssignableToTypeNameOrDefault("FromQueryAttribute", TypeNameStyle.Name);

                string bodyParameterName = fromBodyAttribute.TryGetPropertyValue<string>("Name") ?? parameterName;
                string uriParameterName = fromUriAttribute.TryGetPropertyValue<string>("Name") ?? parameterName;

                var uriParameterNameLower = uriParameterName.ToLowerInvariant();
                OpenApiParameter operationParameter;

                var lowerHttpPath = httpPath.ToLowerInvariant();
                if (lowerHttpPath.Contains("{" + uriParameterNameLower + "}") ||
                    lowerHttpPath.Contains("{" + uriParameterNameLower + ":")) // path parameter
                {
                    operationParameter = context.DocumentGenerator.CreatePrimitiveParameter(uriParameterName, contextualParameter);
                    operationParameter.Kind = OpenApiParameterKind.Path;
                    operationParameter.IsRequired = true; // Path is always required => property not needed

                    if (_settings.SchemaType == SchemaType.Swagger2)
                    {
                        operationParameter.IsNullableRaw = false;
                    }

                    context.OperationDescription.Operation.Parameters.Add(operationParameter);
                }
                else
                {
                    var parameterInfo = _settings.ReflectionService.GetDescription(contextualParameter, _settings);

                    operationParameter = TryAddFileParameter(context, parameterInfo, contextualParameter);
                    if (operationParameter == null)
                    {
                        if (fromRouteAttribute != null)
                        {
                            parameterName = !string.IsNullOrEmpty(fromRouteAttribute.Name) ? fromRouteAttribute.Name : contextualParameter.Name;

                            operationParameter = context.DocumentGenerator.CreatePrimitiveParameter(parameterName, contextualParameter);
                            operationParameter.Kind = OpenApiParameterKind.Path;
                            operationParameter.IsNullableRaw = false;
                            operationParameter.IsRequired = true;

                            context.OperationDescription.Operation.Parameters.Add(operationParameter);
                        }
                        else if (fromHeaderAttribute != null)
                        {
                            parameterName = !string.IsNullOrEmpty(fromHeaderAttribute.Name) ? fromHeaderAttribute.Name : contextualParameter.Name;

                            operationParameter = context.DocumentGenerator.CreatePrimitiveParameter(parameterName, contextualParameter);
                            operationParameter.Kind = OpenApiParameterKind.Header;

                            context.OperationDescription.Operation.Parameters.Add(operationParameter);
                        }
                        else if (fromFormAttribute != null)
                        {
                            operationParameter = context.DocumentGenerator.CreatePrimitiveParameter(parameterName, contextualParameter);
                            operationParameter.Kind = OpenApiParameterKind.FormData;

                            context.OperationDescription.Operation.Parameters.Add(operationParameter);
                        }
                        else
                        {
                            if (parameterInfo.IsComplexType)
                            {
                                // Check for a custom ParameterBindingAttribute (OWIN/WebAPI only)
                                var parameterBindingAttribute = contextualParameter.ContextAttributes.FirstAssignableToTypeNameOrDefault("ParameterBindingAttribute", TypeNameStyle.Name);
                                if (parameterBindingAttribute != null && fromBodyAttribute == null && fromUriAttribute == null && !_settings.IsAspNetCore)
                                {
                                    // If binding attribute is defined in GET method context, it cannot be body parameter and path and form attributes were checked earlier
                                    if (context.OperationDescription.Method == OpenApiOperationMethod.Get)
                                    {
                                        operationParameter = AddPrimitiveParameter(uriParameterName, context, contextualParameter);
                                    } 
                                    else
                                    {
                                        // Try to find a [WillReadBody] attribute on either the action parameter or the bindingAttribute's class
                                        var willReadBodyAttribute = contextualParameter.ContextAttributes.Concat(parameterBindingAttribute.GetType().GetTypeInfo().GetCustomAttributes())
                                            .FirstAssignableToTypeNameOrDefault("WillReadBodyAttribute", TypeNameStyle.Name);

                                        if (willReadBodyAttribute == null)
                                        {
                                            operationParameter = AddBodyParameter(context, bodyParameterName, contextualParameter);
                                        }
                                        else
                                        {
                                            // Try to get a boolean property value from the attribute which explicity tells us whether to read from the body
                                            // If no such property exists, then default to false since WebAPI's HttpParameterBinding.WillReadBody defaults to false
                                            var willReadBody = willReadBodyAttribute.TryGetPropertyValue("WillReadBody", true);
                                            if (willReadBody)
                                            {
                                                operationParameter = AddBodyParameter(context, bodyParameterName, contextualParameter);
                                            }
                                            else
                                            {
                                                // If we are not reading from the body, then treat this as a primitive.
                                                // This may seem odd, but it allows for primitive -> custom complex-type bindings which are very common
                                                // In this case, the API author should use a TypeMapper to define the parameter
                                                operationParameter = AddPrimitiveParameter(uriParameterName, context, contextualParameter);
                                            }
                                        }
                                    }
                                }
                                else if (fromBodyAttribute != null || (fromUriAttribute == null && _settings.IsAspNetCore == false))
                                {
                                    operationParameter = AddBodyParameter(context, bodyParameterName, contextualParameter);
                                }
                                else
                                {
                                    operationParameter = AddPrimitiveParametersFromUri(
                                        context, httpPath, uriParameterName, contextualParameter, parameterInfo);
                                }
                            }
                            else
                            {
                                if (fromBodyAttribute != null)
                                {
                                    operationParameter = AddBodyParameter(context, bodyParameterName, contextualParameter);
                                }
                                else
                                {
                                    operationParameter = AddPrimitiveParameter(uriParameterName, context, contextualParameter);
                                }
                            }
                        }
                    }
                }

                if (operationParameter != null)
                {
                    operationParameter.Position = position;
                    position++;

                    if (_settings.SchemaType == SchemaType.OpenApi3)
                    {
                        operationParameter.IsNullableRaw = null;
                    }

                    if (operationParameter.Name != contextualParameter.ParameterInfo.Name)
                    {
                        operationParameter.OriginalName = contextualParameter.ParameterInfo.Name;
                    }
                    
                    ((Dictionary<ParameterInfo, OpenApiParameter>)context.Parameters)[contextualParameter.ParameterInfo] = operationParameter;
                }
            }

            if (_settings.AddMissingPathParameters)
            {
                foreach (Match match in Regex.Matches(httpPath, "{(.*?)(:(([^/]*)?))?}"))
                {
                    var parameterName = match.Groups[1].Value;
                    if (context.OperationDescription.Operation.Parameters.All(p => !string.Equals(p.Name, parameterName, StringComparison.OrdinalIgnoreCase)))
                    {
                        var parameterType = match.Groups.Count == 5 ? match.Groups[3].Value : "string";
                        var operationParameter = context.DocumentGenerator.CreateUntypedPathParameter(parameterName, parameterType);
                        context.OperationDescription.Operation.Parameters.Add(operationParameter);
                    }
                }
            }

            ApplyOpenApiBodyParameterAttribute(context.OperationDescription, context.MethodInfo);
            RemoveUnusedPathParameters(context.OperationDescription, httpPath);
            UpdateConsumedTypes(context.OperationDescription);
            UpdateNullableRawOperationParameters(context.OperationDescription, _settings.SchemaType);

            EnsureSingleBodyParameter(context.OperationDescription);

            return true;
        }


        private void ApplyOpenApiBodyParameterAttribute(OpenApiOperationDescription operationDescription, MethodInfo methodInfo)
        {
            dynamic bodyParameterAttribute = methodInfo.GetCustomAttributes()
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
                            Type = _settings.SchemaType == SchemaType.Swagger2 ? JsonObjectType.File : JsonObjectType.String,
                            Format = _settings.SchemaType == SchemaType.Swagger2 ? null : JsonFormatStrings.Binary,
                        }
                    };
                }
            }
        }

        /// <summary>
        /// Sets the IsNullableRaw property of parameters to null for OpenApi3 schemas.
        /// </summary>
        /// <param name="operationDescription">Operation to check.</param>
        /// <param name="schemaType">Schema type.</param>
        private void UpdateNullableRawOperationParameters(OpenApiOperationDescription operationDescription, SchemaType schemaType)
        {
            if (schemaType == SchemaType.OpenApi3)
            {
                foreach (OpenApiParameter openApiParameter in operationDescription.Operation.Parameters)
                {
                    openApiParameter.IsNullableRaw = null;
                }
            }
        }

        private void EnsureSingleBodyParameter(OpenApiOperationDescription operationDescription)
        {
            if (operationDescription.Operation.ActualParameters.Count(p => p.Kind == OpenApiParameterKind.Body) > 1)
            {
                throw new InvalidOperationException("The operation '" + operationDescription.Operation.OperationId + "' has more than one body parameter.");
            }
        }

        private void UpdateConsumedTypes(OpenApiOperationDescription operationDescription)
        {
            if (operationDescription.Operation.ActualParameters.Any(p => p.IsBinary || p.ActualSchema.IsBinary))
            {
                operationDescription.Operation.TryAddConsumes("multipart/form-data");
            }
        }

        private void RemoveUnusedPathParameters(OpenApiOperationDescription operationDescription, string httpPath)
        {
            operationDescription.Path = Regex.Replace(httpPath, "{(.*?)(:(([^/]*)?))?}", match =>
            {
                var parameterName = match.Groups[1].Value.TrimEnd('?');
                if (operationDescription.Operation.ActualParameters.Any(p => p.Kind == OpenApiParameterKind.Path && string.Equals(p.Name, parameterName, StringComparison.OrdinalIgnoreCase)))
                {
                    return "{" + parameterName + "}";
                }

                return string.Empty;
            }).TrimEnd('/');
        }

        private OpenApiParameter TryAddFileParameter(
            OperationProcessorContext context, JsonTypeDescription typeInfo, ContextualParameterInfo contextualParameter)
        {
            var isFileArray = IsFileArray(contextualParameter.Type, typeInfo);
            var hasSwaggerFileAttribute = contextualParameter.Attributes
                .FirstAssignableToTypeNameOrDefault("SwaggerFileAttribute", TypeNameStyle.Name) != null;

            if (typeInfo.Type == JsonObjectType.File ||
                typeInfo.Format == JsonFormatStrings.Binary ||
                hasSwaggerFileAttribute ||
                isFileArray)
            {
                return AddFileParameter(context, contextualParameter, isFileArray);
            }

            return null;
        }

        private OpenApiParameter AddFileParameter(OperationProcessorContext context, ContextualParameterInfo contextualParameter, bool isFileArray)
        {
            // TODO: Check if there is a way to control the property name
            var parameterDocumentation = contextualParameter.GetDescription(_settings);
            var operationParameter = context.DocumentGenerator.CreatePrimitiveParameter(
                contextualParameter.Name, parameterDocumentation, contextualParameter);

            InitializeFileParameter(operationParameter, isFileArray);
            context.OperationDescription.Operation.Parameters.Add(operationParameter);
            return operationParameter;
        }

        private bool IsFileArray(Type type, JsonTypeDescription typeInfo)
        {
            var isFormFileCollection = type.Name == "IFormFileCollection";
            if (isFormFileCollection)
            {
                return true;
            }

            if (typeInfo.Type == JsonObjectType.Array && type.GenericTypeArguments.Any())
            {
                var description = _settings.ReflectionService.GetDescription(type.GenericTypeArguments[0].ToContextualType(), _settings);
                if (description.Type == JsonObjectType.File ||
                    description.Format == JsonFormatStrings.Binary)
                {
                    return true;
                }
            }

            return false;
        }

        private OpenApiParameter AddBodyParameter(OperationProcessorContext context, string name, ContextualParameterInfo contextualParameter)
        {
            OpenApiParameter operationParameter;

            var typeDescription = _settings.ReflectionService.GetDescription(contextualParameter, _settings);
            var isRequired = _settings.AllowNullableBodyParameters == false || contextualParameter.ContextAttributes.FirstAssignableToTypeNameOrDefault("RequiredAttribute", TypeNameStyle.Name) != null;
            var isNullable = _settings.AllowNullableBodyParameters && (typeDescription.IsNullable && !isRequired);

            var operation = context.OperationDescription.Operation;
            if (contextualParameter.TypeName == "XmlDocument" || contextualParameter.Type.InheritsFromTypeName("XmlDocument", TypeNameStyle.Name))
            {
                operation.TryAddConsumes("application/xml");
                operationParameter = new OpenApiParameter
                {
                    Name = name,
                    Kind = OpenApiParameterKind.Body,
                    Schema = new JsonSchema
                    {
                        Type = JsonObjectType.String,
                        IsNullableRaw = isNullable
                    },
                    IsNullableRaw = isNullable,
                    IsRequired = isRequired,
                    Description = contextualParameter.GetDescription(_settings)
                };
                operation.Parameters.Add(operationParameter);
            }
            else if (contextualParameter.Type.IsAssignableToTypeName("System.IO.Stream", TypeNameStyle.FullName))
            {
                operation.TryAddConsumes("application/octet-stream");
                operationParameter = new OpenApiParameter
                {
                    Name = name,
                    Kind = OpenApiParameterKind.Body,
                    Schema = new JsonSchema
                    {
                        Type = JsonObjectType.String,
                        Format = JsonFormatStrings.Binary,
                        IsNullableRaw = isNullable
                    },
                    IsNullableRaw = isNullable,
                    IsRequired = isRequired,
                    Description = contextualParameter.GetDescription(_settings)
                };
                operation.Parameters.Add(operationParameter);
            }
            else
            {
                operationParameter = new OpenApiParameter
                {
                    Name = name,
                    Kind = OpenApiParameterKind.Body,
                    IsRequired = isRequired,
                    IsNullableRaw = isNullable,
                    Description = contextualParameter.GetDescription(_settings),
                    Schema = context.SchemaGenerator.GenerateWithReferenceAndNullability<JsonSchema>(
                        contextualParameter, isNullable, schemaResolver: context.SchemaResolver)
                };
                operation.Parameters.Add(operationParameter);
            }

            return operationParameter;
        }

        private OpenApiParameter AddPrimitiveParametersFromUri(
            OperationProcessorContext context, string httpPath, string name, ContextualParameterInfo contextualParameter, JsonTypeDescription typeDescription)
        {
            var operation = context.OperationDescription.Operation;

            if (typeDescription.Type.HasFlag(JsonObjectType.Array))
            {
                var parameterDocumentation = contextualParameter.GetDescription(_settings);
                var operationParameter = context.DocumentGenerator.CreatePrimitiveParameter(
                    name, parameterDocumentation, contextualParameter);

                operationParameter.Kind = OpenApiParameterKind.Query;
                operation.Parameters.Add(operationParameter);
                return operationParameter;
            }
            else
            {
                foreach (var contextualProperty in contextualParameter.Type.GetContextualProperties())
                {
                    if (contextualProperty.ContextAttributes.Select(a => a.GetType()).All(a =>
                        !a.IsAssignableToTypeName("SwaggerIgnoreAttribute", TypeNameStyle.Name) &&
                        !a.IsAssignableToTypeName("JsonIgnoreAttribute", TypeNameStyle.Name)))
                    {
                        var fromQueryAttribute = contextualProperty.ContextAttributes.SingleOrDefault(a => a.GetType().Name == "FromQueryAttribute");
                        var propertyName = fromQueryAttribute.TryGetPropertyValue<string>("Name") ??
                            context.SchemaGenerator.GetPropertyName(null, contextualProperty);

                        dynamic fromRouteAttribute = contextualProperty.ContextAttributes.SingleOrDefault(a => a.GetType().FullName == "Microsoft.AspNetCore.Mvc.FromRouteAttribute");
                        if (fromRouteAttribute != null && !string.IsNullOrEmpty(fromRouteAttribute?.Name))
                        {
                            propertyName = fromRouteAttribute?.Name;
                        }

                        dynamic fromHeaderAttribute = contextualProperty.ContextAttributes.SingleOrDefault(a => a.GetType().FullName == "Microsoft.AspNetCore.Mvc.FromHeaderAttribute");
                        if (fromHeaderAttribute != null && !string.IsNullOrEmpty(fromHeaderAttribute?.Name))
                        {
                            propertyName = fromHeaderAttribute?.Name;
                        }

                        var propertySummary = contextualProperty.PropertyInfo.GetXmlDocsSummary(_settings.GetXmlDocsOptions());
                        var operationParameter = context.DocumentGenerator.CreatePrimitiveParameter(propertyName, propertySummary, contextualProperty.AccessorType);

                        // TODO: Check if required can be controlled with mechanisms other than RequiredAttribute

                        var parameterInfo = _settings.ReflectionService.GetDescription(contextualProperty.AccessorType, _settings);
                        var isFileArray = IsFileArray(contextualProperty.AccessorType.Type, parameterInfo);

                        if (parameterInfo.Type == JsonObjectType.File || isFileArray)
                        {
                            InitializeFileParameter(operationParameter, isFileArray);
                        }
                        else if (fromRouteAttribute != null
                            || httpPath.ToLowerInvariant().Contains("{" + propertyName.ToLower() + "}")
                            || httpPath.ToLowerInvariant().Contains("{" + propertyName.ToLower() + ":"))
                        {
                            operationParameter.Kind = OpenApiParameterKind.Path;
                            operationParameter.IsNullableRaw = false;
                            operationParameter.IsRequired = true; // Path is always required => property not needed
                        }
                        else if (fromHeaderAttribute != null)
                        {
                            operationParameter.Kind = OpenApiParameterKind.Header;
                        }
                        else
                        {
                            operationParameter.Kind = OpenApiParameterKind.Query;
                        }

                        operation.Parameters.Add(operationParameter);
                    }
                }

                return null;
            }
        }

        private OpenApiParameter AddPrimitiveParameter(
            string name, OperationProcessorContext context, ContextualParameterInfo contextualParameter)
        {
            var operationParameter = context.DocumentGenerator.CreatePrimitiveParameter(name, contextualParameter);
            operationParameter.Kind = OpenApiParameterKind.Query;
            operationParameter.IsRequired = operationParameter.IsRequired || contextualParameter.ParameterInfo.HasDefaultValue == false;

            if (contextualParameter.ParameterInfo.HasDefaultValue)
            {
                var defaultValue = context.SchemaGenerator.ConvertDefaultValue(
                    contextualParameter, contextualParameter.ParameterInfo.DefaultValue);

                if (_settings.SchemaType == SchemaType.Swagger2)
                {
                    operationParameter.Default = defaultValue;
                }
                else if (operationParameter.Schema.HasReference)
                {
                    operationParameter.Schema = new JsonSchema
                    {
                        Default = defaultValue,
                        OneOf = { operationParameter.Schema }
                    };
                }
                else
                {
                    operationParameter.Schema.Default = defaultValue;
                }
            }

            context.OperationDescription.Operation.Parameters.Add(operationParameter);
            return operationParameter;
        }

        private void InitializeFileParameter(OpenApiParameter operationParameter, bool isFileArray)
        {
            operationParameter.Type = JsonObjectType.File;
            operationParameter.Kind = OpenApiParameterKind.FormData;

            if (isFileArray)
            {
                operationParameter.CollectionFormat = OpenApiParameterCollectionFormat.Multi;
            }
        }
    }
}
