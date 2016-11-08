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
using NJsonSchema;
using NJsonSchema.Infrastructure;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors
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
        /// <param name="document">The Swagger document.</param>
        /// <param name="operationDescription">The operation description.</param>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="swaggerGenerator">The swagger generator.</param>
        /// <param name="allOperationDescriptions">All operation descriptions.</param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public bool Process(SwaggerDocument document, SwaggerOperationDescription operationDescription, MethodInfo methodInfo,
            SwaggerGenerator swaggerGenerator, IList<SwaggerOperationDescription> allOperationDescriptions)
        {
            var httpPath = operationDescription.Path;
            var parameters = methodInfo.GetParameters().ToList();
            foreach (var parameter in parameters.Where(p => p.ParameterType != typeof(CancellationToken) &&
                                                            p.GetCustomAttributes().All(a => a.GetType().Name != "FromServicesAttribute") &&
                                                            p.GetCustomAttributes().All(a => a.GetType().Name != "BindNeverAttribute")))
            {
                var nameLower = parameter.Name.ToLowerInvariant();
                if (httpPath.ToLowerInvariant().Contains("{" + nameLower + "}") ||
                    httpPath.ToLowerInvariant().Contains("{" + nameLower + ":")) // path parameter
                {
                    var operationParameter = swaggerGenerator.CreatePrimitiveParameter(parameter.Name, parameter);
                    operationParameter.Kind = SwaggerParameterKind.Path;
                    operationParameter.IsNullableRaw = false;
                    operationParameter.IsRequired = true; // Path is always required => property not needed

                    operationDescription.Operation.Parameters.Add(operationParameter);
                }
                else
                {
                    var parameterInfo = JsonObjectTypeDescription.FromType(parameter.ParameterType, parameter.GetCustomAttributes(), _settings.DefaultEnumHandling);
                    if (TryAddFileParameter(parameterInfo, operationDescription.Operation, parameter, swaggerGenerator) == false)
                    {
                        dynamic fromBodyAttribute = parameter.GetCustomAttributes()
                            .SingleOrDefault(a => a.GetType().Name == "FromBodyAttribute");

                        dynamic fromUriAttribute = parameter.GetCustomAttributes()
                            .SingleOrDefault(a => a.GetType().Name == "FromUriAttribute" || a.GetType().Name == "FromQueryAttribute");

                        var bodyParameterName = TryGetStringPropertyValue(fromBodyAttribute, "Name") ?? parameter.Name;
                        var uriParameterName = TryGetStringPropertyValue(fromUriAttribute, "Name") ?? parameter.Name;

                        if (parameterInfo.IsComplexType)
                        {
                            if (fromBodyAttribute != null || (fromUriAttribute == null && _settings.IsAspNetCore == false))
                                AddBodyParameter(bodyParameterName, parameter, operationDescription.Operation, swaggerGenerator);
                            else
                                AddPrimitiveParametersFromUri(uriParameterName, operationDescription.Operation, parameter, parameterInfo, swaggerGenerator);
                        }
                        else
                        {
                            if (fromBodyAttribute != null)
                                AddBodyParameter(bodyParameterName, parameter, operationDescription.Operation, swaggerGenerator);
                            else
                                AddPrimitiveParameter(uriParameterName, operationDescription.Operation, parameter, swaggerGenerator);
                        }
                    }
                }
            }

            if (_settings.AddMissingPathParameters)
            {
                foreach (Match match in Regex.Matches(httpPath, "{(.*?)(:(.*?))?}"))
                {
                    var parameterName = match.Groups[1].Value;
                    if (operationDescription.Operation.Parameters.All(p => p.Name != parameterName))
                    {
                        var parameterType = match.Groups.Count == 4 ? match.Groups[3].Value : "string";
                        var operationParameter = swaggerGenerator.CreatePathParameter(parameterName, parameterType);
                        operationDescription.Operation.Parameters.Add(operationParameter);
                    }
                }
            }

            RemoveUnusedPathParameters(operationDescription, httpPath);
            UpdateConsumedTypes(operationDescription);

            EnsureSingleBodyParameter(operationDescription);

            return true;
        }

        private void EnsureSingleBodyParameter(SwaggerOperationDescription operationDescription)
        {
            if (operationDescription.Operation.ActualParameters.Count(p => p.Kind == SwaggerParameterKind.Body) > 1)
                throw new InvalidOperationException("The operation '" + operationDescription.Operation.OperationId + "' has more than one body parameter.");
        }

        private void UpdateConsumedTypes(SwaggerOperationDescription operationDescription)
        {
            if (operationDescription.Operation.ActualParameters.Any(p => p.Type == JsonObjectType.File))
                operationDescription.Operation.Consumes = new List<string> { "multipart/form-data" };
        }

        private void RemoveUnusedPathParameters(SwaggerOperationDescription operationDescription, string httpPath)
        {
            operationDescription.Path = Regex.Replace(httpPath, "{(.*?)(:(.*?))?}", match =>
            {
                var parameterName = match.Groups[1].Value.TrimEnd('?');
                if (operationDescription.Operation.ActualParameters.Any(p => p.Kind == SwaggerParameterKind.Path && p.Name == parameterName))
                    return "{" + parameterName + "}";
                return string.Empty;
            }).TrimEnd('/');
        }

        private bool TryAddFileParameter(JsonObjectTypeDescription info, SwaggerOperation operation, ParameterInfo parameter, SwaggerGenerator swaggerGenerator)
        {
            var isFileArray = IsFileArray(parameter.ParameterType, info);
            if (info.Type == JsonObjectType.File || isFileArray)
            {
                AddFileParameter(parameter, isFileArray, operation, swaggerGenerator);
                return true;
            }

            return false;
        }

        private void AddFileParameter(ParameterInfo parameter, bool isFileArray, SwaggerOperation operation, SwaggerGenerator swaggerGenerator)
        {
            var attributes = parameter.GetCustomAttributes().ToList();

            // TODO: Check if there is a way to control the property name
            var operationParameter = swaggerGenerator.CreatePrimitiveParameter(parameter.Name, parameter.GetXmlDocumentation(), parameter.ParameterType, attributes);

            InitializeFileParameter(operationParameter, isFileArray);
            operation.Parameters.Add(operationParameter);
        }

        private bool IsFileArray(Type type, JsonObjectTypeDescription typeInfo)
        {
            var isFormFileCollection = type.Name == "IFormFileCollection";
            var isFileArray = typeInfo.Type == JsonObjectType.Array && type.GenericTypeArguments.Any() &&
                              JsonObjectTypeDescription.FromType(type.GenericTypeArguments[0], null, _settings.DefaultEnumHandling).Type == JsonObjectType.File;
            return isFormFileCollection || isFileArray;
        }

        private void AddBodyParameter(string name, ParameterInfo parameter, SwaggerOperation operation, SwaggerGenerator swaggerGenerator)
        {
            var operationParameter = swaggerGenerator.CreateBodyParameter(name, parameter);
            operation.Parameters.Add(operationParameter);
        }

        private void AddPrimitiveParametersFromUri(string name, SwaggerOperation operation, ParameterInfo parameter, JsonObjectTypeDescription typeDescription, SwaggerGenerator swaggerGenerator)
        {
            if (typeDescription.Type.HasFlag(JsonObjectType.Array))
            {
                var operationParameter = swaggerGenerator.CreatePrimitiveParameter(name,
                    parameter.GetXmlDocumentation(), parameter.ParameterType.GetEnumerableItemType(), parameter.GetCustomAttributes().ToList());

                operationParameter.Kind = SwaggerParameterKind.Query;
                operationParameter.CollectionFormat = SwaggerParameterCollectionFormat.Multi;
                operation.Parameters.Add(operationParameter);
            }
            else
            {
                foreach (var property in parameter.ParameterType.GetRuntimeProperties())
                {
                    var attributes = property.GetCustomAttributes().ToList();
                    var fromQueryAttribute = attributes.SingleOrDefault(a => a.GetType().Name == "FromQueryAttribute");

                    var propertyName = TryGetStringPropertyValue(fromQueryAttribute, "Name") ?? JsonPathUtilities.GetPropertyName(property, _settings.DefaultPropertyNameHandling);
                    var operationParameter = swaggerGenerator.CreatePrimitiveParameter(propertyName, property.GetXmlSummary(), property.PropertyType, attributes);

                    // TODO: Check if required can be controlled with mechanisms other than RequiredAttribute

                    var parameterInfo = JsonObjectTypeDescription.FromType(property.PropertyType, attributes, _settings.DefaultEnumHandling);
                    var isFileArray = IsFileArray(property.PropertyType, parameterInfo);
                    if (parameterInfo.Type == JsonObjectType.File || isFileArray)
                        InitializeFileParameter(operationParameter, isFileArray);
                    else
                        operationParameter.Kind = SwaggerParameterKind.Query;

                    operation.Parameters.Add(operationParameter);
                }
            }
        }

        private void AddPrimitiveParameter(string name, SwaggerOperation operation, ParameterInfo parameter, SwaggerGenerator swaggerGenerator)
        {
            var operationParameter = swaggerGenerator.CreatePrimitiveParameter(name, parameter);
            operationParameter.Kind = SwaggerParameterKind.Query;
            operationParameter.IsRequired = operationParameter.IsRequired || parameter.HasDefaultValue == false;
            operation.Parameters.Add(operationParameter);
        }

        private void InitializeFileParameter(SwaggerParameter operationParameter, bool isFileArray)
        {
            operationParameter.Type = JsonObjectType.File;
            operationParameter.Kind = SwaggerParameterKind.FormData;

            if (isFileArray)
                operationParameter.CollectionFormat = SwaggerParameterCollectionFormat.Multi;
        }

        private string TryGetStringPropertyValue(dynamic obj, string propertyName)
        {
            return ((object)obj)?.GetType().GetRuntimeProperty(propertyName) != null && !string.IsNullOrEmpty(obj.Name) ? obj.Name : null;
        }
    }
}