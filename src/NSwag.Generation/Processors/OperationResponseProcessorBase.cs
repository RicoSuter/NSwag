﻿//-----------------------------------------------------------------------
// <copyright file="OperationResponseProcessorBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Reflection;
using System.Xml.Linq;
using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Infrastructure;
using NSwag.Generation.Processors.Contexts;

namespace NSwag.Generation.Processors
{
    /// <summary>The OperationResponseProcessor base class.</summary>
    public abstract class OperationResponseProcessorBase
    {
        private readonly OpenApiDocumentGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="OperationResponseProcessorBase"/> class.</summary>
        /// <param name="settings">The settings.</param>
        public OperationResponseProcessorBase(OpenApiDocumentGeneratorSettings settings)
        {
            _settings = settings;
        }

        /// <summary>Gets the response HTTP status code for an empty/void response and the given generator.</summary>
        /// <returns>The status code.</returns>
        protected abstract string GetVoidResponseStatusCode();

        /// <summary>Generates the responses based on the given return type attributes.</summary>
        /// <param name="operationProcessorContext">The context.</param>
        /// <param name="responseTypeAttributes">The response type attributes.</param>
        /// <returns>The task.</returns>
        public void ProcessResponseTypeAttributes(OperationProcessorContext operationProcessorContext, IEnumerable<Attribute> responseTypeAttributes)
        {
            var returnParameter = operationProcessorContext.MethodInfo.ReturnParameter;

            var successResponseDescription = returnParameter
                .ToContextualParameter()
                .GetDescription(_settings.SchemaSettings) ?? string.Empty;

            var responseDescriptions = GetOperationResponseDescriptions(responseTypeAttributes, successResponseDescription);
            ProcessOperationDescriptions(responseDescriptions, returnParameter, operationProcessorContext, successResponseDescription);
        }

        /// <summary>Updates the response description based on the return parameter or the response tags in the method's xml docs.</summary>
        /// <param name="operationProcessorContext">The context.</param>
        /// <returns>The task.</returns>
        protected void UpdateResponseDescription(OperationProcessorContext operationProcessorContext)
        {
            if (operationProcessorContext.MethodInfo == null)
            {
                return;
            }

            var returnParameter = operationProcessorContext.MethodInfo.ReturnParameter.ToContextualParameter();

            var returnParameterXmlDocs = returnParameter.GetDescription(_settings.SchemaSettings) ?? string.Empty;
            var operationXmlDocsNodes = GetResponseXmlDocsNodes(operationProcessorContext.MethodInfo);

            if (!string.IsNullOrEmpty(returnParameterXmlDocs) || operationXmlDocsNodes?.Any() == true)
            {
                foreach (var response in operationProcessorContext.OperationDescription.Operation._responses)
                {
                    if (string.IsNullOrEmpty(response.Value.Description))
                    {
                        // Support for <response code="201">Order created</response> tags
                        var responseXmlDocs = GetResponseXmlDocsElement(operationProcessorContext.MethodInfo, response.Key)?.Value;
                        if (!string.IsNullOrEmpty(responseXmlDocs))
                        {
                            response.Value.Description = responseXmlDocs;
                        }
                        else if (!string.IsNullOrEmpty(returnParameterXmlDocs) && HttpUtilities.IsSuccessStatusCode(response.Key))
                        {
                            response.Value.Description = returnParameterXmlDocs;
                        }
                    }
                }
            }
        }

        /// <summary>Gets the XML documentation element for the given response code or null.</summary>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="responseCode">The response code.</param>
        /// <returns>The XML element or null.</returns>
        protected XElement GetResponseXmlDocsElement(MethodInfo methodInfo, string responseCode)
        {
            var operationXmlDocsNodes = GetResponseXmlDocsNodes(methodInfo);
            try
            {
                return operationXmlDocsNodes?.SingleOrDefault(n => n.Name == "response" && n.Attributes().Any(a => a.Name == "code" && a.Value == responseCode));
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Multiple response tags with code '{responseCode}' found in XML documentation for method '{methodInfo.Name}'.", ex);
            }
        }

        private IEnumerable<XElement> GetResponseXmlDocsNodes(MethodInfo methodInfo)
        {
            var operationXmlDocs = methodInfo?.GetXmlDocsElement(_settings.SchemaSettings.GetXmlDocsOptions());
            return operationXmlDocs?.Nodes()?.OfType<XElement>();
        }

        private List<OperationResponseDescription> GetOperationResponseDescriptions(IEnumerable<Attribute> responseTypeAttributes, string successResponseDescription)
        {
            List<OperationResponseDescription> operationResponseDescriptions = [];
            foreach (var attribute in responseTypeAttributes)
            {
                dynamic responseTypeAttribute = attribute;
                var attributeType = attribute.GetType();

                var isProducesAttributeWithNoType = // ignore ProducesAttribute if it has no type, https://github.com/RicoSuter/NSwag/issues/1201
                    attributeType.Name == "ProducesAttribute" && attribute.HasProperty("Type") && responseTypeAttribute.Type == null;

                if (!isProducesAttributeWithNoType)
                {
                    var returnType = typeof(void);
                    if (attributeType.GetRuntimeProperty("ResponseType") != null)
                    {
                        returnType = responseTypeAttribute.ResponseType;
                    }
                    else if (attributeType.GetRuntimeProperty("Type") != null)
                    {
                        returnType = responseTypeAttribute.Type;
                    }

                    if (returnType == null)
                    {
                        returnType = typeof(void);
                    }

                    var httpStatusCode = IsVoidResponse(returnType) ? GetVoidResponseStatusCode() : "200";
                    if (attributeType.GetRuntimeProperty("HttpStatusCode") != null && responseTypeAttribute.HttpStatusCode != null)
                    {
                        httpStatusCode = responseTypeAttribute.HttpStatusCode.ToString();
                    }
                    else if (attributeType.GetRuntimeProperty("StatusCode") != null && responseTypeAttribute.StatusCode != null)
                    {
                        httpStatusCode = responseTypeAttribute.StatusCode.ToString();
                    }

                    var description = HttpUtilities.IsSuccessStatusCode(httpStatusCode) ? successResponseDescription : string.Empty;
                    if (attributeType.GetRuntimeProperty("Description") != null)
                    {
                        if (!string.IsNullOrEmpty(responseTypeAttribute.Description))
                        {
                            description = responseTypeAttribute.Description;
                        }
                    }

                    var isNullable = true;
                    if (attributeType.GetRuntimeProperty("IsNullable") != null)
                    {
                        isNullable = responseTypeAttribute.IsNullable;
                    }

                    operationResponseDescriptions.Add(new OperationResponseDescription(httpStatusCode, returnType, isNullable, description));
                }
            }

            return operationResponseDescriptions;
        }

        private void ProcessOperationDescriptions(List<OperationResponseDescription> operationDescriptions, ParameterInfo returnParameter, OperationProcessorContext context, string successResponseDescription)
        {
            foreach (var statusCodeGroup in operationDescriptions.GroupBy(r => r.StatusCode))
            {
                var httpStatusCode = statusCodeGroup.Key;

                var returnType = statusCodeGroup.Select(r => r.ResponseType).GetCommonBaseType();
                var returnParameterAttributes = returnParameter?.GetCustomAttributes(false)?.OfType<Attribute>();
                var contextualReturnType = returnType.ToContextualType(returnParameterAttributes);

                var description = string.Join("\nor\n", statusCodeGroup.Select(r => r.Description));

                var response = new OpenApiResponse
                {
                    Description = description ?? string.Empty
                };

                if (!IsVoidResponse(returnType))
                {
                    response.ExpectedSchemas = GenerateExpectedSchemas(statusCodeGroup, context);

                    var nullableXmlAttribute = GetResponseXmlDocsElement(context.MethodInfo, httpStatusCode)?.Attribute("nullable");

                    var isResponseNullable = nullableXmlAttribute != null ?
                        nullableXmlAttribute.Value.Equals("true", StringComparison.OrdinalIgnoreCase) :
                        statusCodeGroup.Any(r => r.IsNullable) &&
                            _settings.SchemaSettings.ReflectionService.GetDescription(contextualReturnType, _settings.DefaultResponseReferenceTypeNullHandling, _settings.SchemaSettings).IsNullable;

                    response.IsNullableRaw = isResponseNullable;
                    response.Schema = context.SchemaGenerator.GenerateWithReferenceAndNullability<JsonSchema>(
                        contextualReturnType, isResponseNullable, context.SchemaResolver);
                }

                context.OperationDescription.Operation.Responses[httpStatusCode] = response;
            }

            bool loadDefaultSuccessResponseFromReturnType;
            if (operationDescriptions.Count > 0)
            {
                // If there are some attributes declared on the controller \ action, only return a default success response
                // if a 2xx status code isn't already defined and the SwaggerDefaultResponseAttribute is declared.
                var operationResponses = context.OperationDescription.Operation._responses;
                var hasSuccessResponse = operationResponses.KeyCollection.Any(HttpUtilities.IsSuccessStatusCode);

                loadDefaultSuccessResponseFromReturnType = !hasSuccessResponse &&
                    context.MethodInfo.GetCustomAttributes()
                        .Any(a => a.GetType().IsAssignableToTypeName("SwaggerDefaultResponseAttribute", TypeNameStyle.Name)) ||
                    context.MethodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes()
                        .Any(a => a.GetType().IsAssignableToTypeName("SwaggerDefaultResponseAttribute", TypeNameStyle.Name));
            }
            else
            {
                // If there are no attributes declared on the controller \ action, always return a success response
                loadDefaultSuccessResponseFromReturnType = true;
            }

            if (loadDefaultSuccessResponseFromReturnType)
            {
                LoadDefaultSuccessResponse(returnParameter, successResponseDescription, context);
            }
        }

        private List<JsonExpectedSchema> GenerateExpectedSchemas(
            IGrouping<string, OperationResponseDescription> group, OperationProcessorContext context)
        {
            if (group.Count() > 1)
            {
                var expectedSchemas = new List<JsonExpectedSchema>();
                foreach (var response in group)
                {
                    var contextualResponseType = response.ResponseType.ToContextualType();

                    var isNullable = _settings.SchemaSettings.ReflectionService.GetDescription(contextualResponseType, _settings.SchemaSettings).IsNullable;
                    var schema = context.SchemaGenerator.GenerateWithReferenceAndNullability<JsonSchema>(
                        contextualResponseType, isNullable, context.SchemaResolver);

                    expectedSchemas.Add(new JsonExpectedSchema
                    {
                        Schema = schema,
                        Description = response.Description
                    });
                }

                return expectedSchemas;
            }
            return null;
        }

        private void LoadDefaultSuccessResponse(ParameterInfo returnParameter, string successXmlDescription, OperationProcessorContext context)
        {
            var operation = context.OperationDescription.Operation;

            var returnType = returnParameter.ParameterType;
            if (returnType == typeof(Task))
            {
                returnType = typeof(void);
            }

            returnType = GenericResultWrapperTypes.RemoveGenericWrapperTypes(
                returnType, t => t.Name, t => t.GenericTypeArguments[0]);

            if (IsVoidResponse(returnType))
            {
                operation.Responses[GetVoidResponseStatusCode()] = new OpenApiResponse
                {
                    Description = successXmlDescription
                };
            }
            else
            {
                var returnParameterAttributes = returnParameter?.GetCustomAttributes(false)?.OfType<Attribute>() ?? [];
                var contextualReturnParameter = returnType.ToContextualType(returnParameterAttributes);

                var typeDescription = _settings.SchemaSettings.ReflectionService.GetDescription(contextualReturnParameter, _settings.SchemaSettings);
                var responseSchema = context.SchemaGenerator.GenerateWithReferenceAndNullability<JsonSchema>(
                    contextualReturnParameter, typeDescription.IsNullable, context.SchemaResolver);

                operation.Responses["200"] = new OpenApiResponse
                {
                    Description = successXmlDescription,
                    IsNullableRaw = typeDescription.IsNullable,
                    Schema = responseSchema
                };
            }
        }

        private static bool IsVoidResponse(Type returnType)
        {
            return returnType == null || returnType.FullName == "System.Void";
        }
    }
}
