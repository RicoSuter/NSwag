//-----------------------------------------------------------------------
// <copyright file="OperationParameterProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Infrastructure;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.Processors.Contexts;

namespace NSwag.SwaggerGeneration.WebApi.Processors
{
    /// <summary>Generates the operation's parameters.</summary>
    public class OperationParameterProcessor : IOperationProcessor
    {
        private readonly WebApiToSwaggerGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="OperationParameterProcessor"/> class.</summary>
        /// <param name="settings">The settings.</param>
        public OperationParameterProcessor(WebApiToSwaggerGeneratorSettings settings)
        {
            _settings = settings;
        }

        /// <summary>Processes the specified method information.</summary>
        /// <param name="context"></param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public async Task<bool> ProcessAsync(OperationProcessorContext context)
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
                SwaggerParameter operationParameter;

                var lowerHttpPath = httpPath.ToLowerInvariant();
                if (lowerHttpPath.Contains("{" + uriParameterNameLower + "}") ||
                    lowerHttpPath.Contains("{" + uriParameterNameLower + ":")) // path parameter
                {
                    operationParameter = await context.SwaggerGenerator.CreatePrimitiveParameterAsync(uriParameterName, contextualParameter).ConfigureAwait(false);
                    operationParameter.Kind = SwaggerParameterKind.Path;
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

                    operationParameter = await TryAddFileParameterAsync(context, parameterInfo, contextualParameter).ConfigureAwait(false);
                    if (operationParameter == null)
                    {
                        if (fromRouteAttribute != null)
                        {
                            parameterName = !string.IsNullOrEmpty(fromRouteAttribute.Name) ? fromRouteAttribute.Name : contextualParameter.Name;

                            operationParameter = await context.SwaggerGenerator.CreatePrimitiveParameterAsync(parameterName, contextualParameter).ConfigureAwait(false);
                            operationParameter.Kind = SwaggerParameterKind.Path;
                            operationParameter.IsNullableRaw = false;
                            operationParameter.IsRequired = true;

                            context.OperationDescription.Operation.Parameters.Add(operationParameter);
                        }
                        else if (fromHeaderAttribute != null)
                        {
                            parameterName = !string.IsNullOrEmpty(fromHeaderAttribute.Name) ? fromHeaderAttribute.Name : contextualParameter.Name;

                            operationParameter = await context.SwaggerGenerator.CreatePrimitiveParameterAsync(parameterName, contextualParameter).ConfigureAwait(false);
                            operationParameter.Kind = SwaggerParameterKind.Header;

                            context.OperationDescription.Operation.Parameters.Add(operationParameter);
                        }
                        else if (fromFormAttribute != null)
                        {
                            operationParameter = await context.SwaggerGenerator.CreatePrimitiveParameterAsync(parameterName, contextualParameter).ConfigureAwait(false);
                            operationParameter.Kind = SwaggerParameterKind.FormData;

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
                                    // Try to find a [WillReadBody] attribute on either the action parameter or the bindingAttribute's class
                                    var willReadBodyAttribute = contextualParameter.ContextAttributes.Concat(parameterBindingAttribute.GetType().GetTypeInfo().GetCustomAttributes())
                                        .FirstAssignableToTypeNameOrDefault("WillReadBodyAttribute", TypeNameStyle.Name);

                                    if (willReadBodyAttribute == null)
                                    {
                                        operationParameter = await AddBodyParameterAsync(context, bodyParameterName, contextualParameter).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        // Try to get a boolean property value from the attribute which explicity tells us whether to read from the body
                                        // If no such property exists, then default to false since WebAPI's HttpParameterBinding.WillReadBody defaults to false
                                        var willReadBody = willReadBodyAttribute.TryGetPropertyValue("WillReadBody", true);
                                        if (willReadBody)
                                        {
                                            operationParameter = await AddBodyParameterAsync(context, bodyParameterName, contextualParameter).ConfigureAwait(false);
                                        }
                                        else
                                        {
                                            // If we are not reading from the body, then treat this as a primitive.
                                            // This may seem odd, but it allows for primitive -> custom complex-type bindings which are very common
                                            // In this case, the API author should use a TypeMapper to define the parameter
                                            operationParameter = await AddPrimitiveParameterAsync(uriParameterName, context, contextualParameter).ConfigureAwait(false);
                                        }
                                    }
                                }
                                else if (fromBodyAttribute != null || (fromUriAttribute == null && _settings.IsAspNetCore == false))
                                {
                                    operationParameter = await AddBodyParameterAsync(context, bodyParameterName, contextualParameter).ConfigureAwait(false);
                                }
                                else
                                {
                                    operationParameter = await AddPrimitiveParametersFromUriAsync(
                                        context, httpPath, uriParameterName, contextualParameter, parameterInfo).ConfigureAwait(false);
                                }
                            }
                            else
                            {
                                if (fromBodyAttribute != null)
                                {
                                    operationParameter = await AddBodyParameterAsync(context, bodyParameterName, contextualParameter).ConfigureAwait(false);
                                }
                                else
                                {
                                    operationParameter = await AddPrimitiveParameterAsync(uriParameterName, context, contextualParameter).ConfigureAwait(false);
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

                    ((Dictionary<ParameterInfo, SwaggerParameter>)context.Parameters)[contextualParameter.ParameterInfo] = operationParameter;
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
                        var operationParameter = context.SwaggerGenerator.CreateUntypedPathParameter(parameterName, parameterType);
                        context.OperationDescription.Operation.Parameters.Add(operationParameter);
                    }
                }
            }

            RemoveUnusedPathParameters(context.OperationDescription, httpPath);
            UpdateConsumedTypes(context.OperationDescription);

            EnsureSingleBodyParameter(context.OperationDescription);

            return true;
        }

        private void EnsureSingleBodyParameter(SwaggerOperationDescription operationDescription)
        {
            if (operationDescription.Operation.ActualParameters.Count(p => p.Kind == SwaggerParameterKind.Body) > 1)
            {
                throw new InvalidOperationException("The operation '" + operationDescription.Operation.OperationId + "' has more than one body parameter.");
            }
        }

        private void UpdateConsumedTypes(SwaggerOperationDescription operationDescription)
        {
            if (operationDescription.Operation.ActualParameters.Any(p => p.IsBinary || p.ActualSchema.IsBinary))
            {
                operationDescription.Operation.TryAddConsumes("multipart/form-data");
            }
        }

        private void RemoveUnusedPathParameters(SwaggerOperationDescription operationDescription, string httpPath)
        {
            operationDescription.Path = Regex.Replace(httpPath, "{(.*?)(:(([^/]*)?))?}", match =>
            {
                var parameterName = match.Groups[1].Value.TrimEnd('?');
                if (operationDescription.Operation.ActualParameters.Any(p => p.Kind == SwaggerParameterKind.Path && string.Equals(p.Name, parameterName, StringComparison.OrdinalIgnoreCase)))
                {
                    return "{" + parameterName + "}";
                }

                return string.Empty;
            }).TrimEnd('/');
        }

        private async Task<SwaggerParameter> TryAddFileParameterAsync(
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
                return await AddFileParameterAsync(context, contextualParameter, isFileArray).ConfigureAwait(false);
            }

            return null;
        }

        private async Task<SwaggerParameter> AddFileParameterAsync(OperationProcessorContext context, ContextualParameterInfo contextualParameter, bool isFileArray)
        {
            // TODO: Check if there is a way to control the property name
            var parameterDocumentation = await contextualParameter.GetDescriptionAsync().ConfigureAwait(false);
            var operationParameter = await context.SwaggerGenerator.CreatePrimitiveParameterAsync(
                contextualParameter.Name, parameterDocumentation, contextualParameter).ConfigureAwait(false);

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

        private async Task<SwaggerParameter> AddBodyParameterAsync(OperationProcessorContext context, string name, ContextualParameterInfo contextualParameter)
        {
            SwaggerParameter operationParameter;

            var typeDescription = _settings.ReflectionService.GetDescription(contextualParameter, _settings);
            var isNullable = _settings.AllowNullableBodyParameters && typeDescription.IsNullable;

            var operation = context.OperationDescription.Operation;
            if (contextualParameter.TypeName == "XmlDocument" || contextualParameter.Type.InheritsFromTypeName("XmlDocument", TypeNameStyle.Name))
            {
                operation.TryAddConsumes("application/xml");
                operationParameter = new SwaggerParameter
                {
                    Name = name,
                    Kind = SwaggerParameterKind.Body,
                    Schema = new JsonSchema
                    {
                        Type = JsonObjectType.String,
                        IsNullableRaw = isNullable
                    },
                    IsNullableRaw = isNullable,
                    IsRequired = contextualParameter.ParameterInfo.HasDefaultValue == false,
                    Description = await contextualParameter.GetDescriptionAsync().ConfigureAwait(false)
                };
                operation.Parameters.Add(operationParameter);
            }
            else if (contextualParameter.Type.IsAssignableToTypeName("System.IO.Stream", TypeNameStyle.FullName))
            {
                operation.TryAddConsumes("application/octet-stream");
                operationParameter = new SwaggerParameter
                {
                    Name = name,
                    Kind = SwaggerParameterKind.Body,
                    Schema = new JsonSchema
                    {
                        Type = JsonObjectType.String,
                        Format = JsonFormatStrings.Byte,
                        IsNullableRaw = isNullable
                    },
                    IsNullableRaw = isNullable,
                    IsRequired = contextualParameter.ParameterInfo.HasDefaultValue == false,
                    Description = await contextualParameter.GetDescriptionAsync().ConfigureAwait(false)
                };
                operation.Parameters.Add(operationParameter);
            }
            else
            {
                operationParameter = new SwaggerParameter
                {
                    Name = name,
                    Kind = SwaggerParameterKind.Body,
                    IsRequired = true, // FromBody parameters are always required
                    IsNullableRaw = isNullable,
                    Description = await contextualParameter.GetDescriptionAsync().ConfigureAwait(false),
                    Schema = await context.SchemaGenerator.GenerateWithReferenceAndNullabilityAsync<JsonSchema>(
                        contextualParameter, isNullable, schemaResolver: context.SchemaResolver).ConfigureAwait(false)
                };
                operation.Parameters.Add(operationParameter);
            }

            return operationParameter;
        }

        private async Task<SwaggerParameter> AddPrimitiveParametersFromUriAsync(
            OperationProcessorContext context, string httpPath, string name, ContextualParameterInfo contextualParameter, JsonTypeDescription typeDescription)
        {
            var operation = context.OperationDescription.Operation;

            if (typeDescription.Type.HasFlag(JsonObjectType.Array))
            {
                var parameterDocumentation = await contextualParameter.GetDescriptionAsync().ConfigureAwait(false);
                var operationParameter = await context.SwaggerGenerator.CreatePrimitiveParameterAsync(
                    name, parameterDocumentation, contextualParameter).ConfigureAwait(false);

                operationParameter.Kind = SwaggerParameterKind.Query;
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
                            context.SchemaGenerator.GetPropertyName(null, contextualProperty.MemberInfo);

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

                        var propertySummary = await contextualProperty.PropertyInfo.GetXmlDocsSummaryAsync().ConfigureAwait(false);
                        var operationParameter = await context.SwaggerGenerator.CreatePrimitiveParameterAsync(propertyName, propertySummary, contextualProperty).ConfigureAwait(false);

                        // TODO: Check if required can be controlled with mechanisms other than RequiredAttribute

                        var parameterInfo = _settings.ReflectionService.GetDescription(contextualProperty, _settings);
                        var isFileArray = IsFileArray(contextualProperty.Type, parameterInfo);

                        if (parameterInfo.Type == JsonObjectType.File || isFileArray)
                        {
                            InitializeFileParameter(operationParameter, isFileArray);
                        }
                        else if (fromRouteAttribute != null
                            || httpPath.ToLowerInvariant().Contains("{" + propertyName.ToLower() + "}")
                            || httpPath.ToLowerInvariant().Contains("{" + propertyName.ToLower() + ":"))
                        {
                            operationParameter.Kind = SwaggerParameterKind.Path;
                            operationParameter.IsNullableRaw = false;
                            operationParameter.IsRequired = true; // Path is always required => property not needed
                        }
                        else if (fromHeaderAttribute != null)
                        {
                            operationParameter.Kind = SwaggerParameterKind.Header;
                        }
                        else
                        {
                            operationParameter.Kind = SwaggerParameterKind.Query;
                        }

                        operation.Parameters.Add(operationParameter);
                    }
                }

                return null;
            }
        }

        private async Task<SwaggerParameter> AddPrimitiveParameterAsync(
            string name, OperationProcessorContext context, ContextualParameterInfo contextualParameter)
        {
            var operationParameter = await context.SwaggerGenerator.CreatePrimitiveParameterAsync(name, contextualParameter).ConfigureAwait(false);
            operationParameter.Kind = SwaggerParameterKind.Query;
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

        private void InitializeFileParameter(SwaggerParameter operationParameter, bool isFileArray)
        {
            operationParameter.Type = JsonObjectType.File;
            operationParameter.Kind = SwaggerParameterKind.FormData;

            if (isFileArray)
            {
                operationParameter.CollectionFormat = SwaggerParameterCollectionFormat.Multi;
            }
        }
    }
}
