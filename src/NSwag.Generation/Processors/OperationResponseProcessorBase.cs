//-----------------------------------------------------------------------
// <copyright file="OperationResponseProcessorBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Namotion.Reflection;
using NJsonSchema;
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
                .GetDescription(_settings) ?? string.Empty;

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

            var returnParameterXmlDocs = returnParameter.GetDescription(_settings) ?? string.Empty;
            var operationXmlDocsNodes = GetResponseXmlDocsNodes(operationProcessorContext.MethodInfo);

            if (!string.IsNullOrEmpty(returnParameterXmlDocs) || operationXmlDocsNodes?.Any() == true)
            {
                foreach (var response in operationProcessorContext.OperationDescription.Operation.Responses)
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
            return operationXmlDocsNodes?.SingleOrDefault(n => n.Name == "response" && n.Attributes().Any(a => a.Name == "code" && a.Value == responseCode));
        }

        private IEnumerable<XElement> GetResponseXmlDocsNodes(MethodInfo methodInfo)
        {
            var operationXmlDocs = methodInfo?.GetXmlDocsElement(_settings.GetXmlDocsOptions());
            return operationXmlDocs?.Nodes()?.OfType<XElement>();
        }

        private IEnumerable<OperationResponseDescription> GetOperationResponseDescriptions(IEnumerable<Attribute> responseTypeAttributes, string successResponseDescription)
        {
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

                    yield return new OperationResponseDescription(httpStatusCode, returnType, isNullable, description);
                }
            }
        }

        private void ProcessOperationDescriptions(IEnumerable<OperationResponseDescription> operationDescriptions, ParameterInfo returnParameter, OperationProcessorContext context, string successResponseDescription)
        {
            foreach (var statusCodeGroup in operationDescriptions.GroupBy(r => r.StatusCode))
            {
                var httpStatusCode = statusCodeGroup.Key;

                var returnType = statusCodeGroup.Select(r => r.ResponseType).GetCommonBaseType();
                var returnParameterAttributes = returnParameter?.GetCustomAttributes(false)?.OfType<Attribute>();
                var contextualReturnType = returnType.ToContextualType(returnParameterAttributes);

                var description = string.Join("\nor\n", statusCodeGroup.Select(r => r.Description));

                var typeDescription = _settings.ReflectionService.GetDescription(
                    contextualReturnType, _settings.DefaultResponseReferenceTypeNullHandling, _settings);

                var response = new OpenApiResponse
                {
                    Description = description ?? string.Empty
                };

                if (IsVoidResponse(returnType) == false)
                {
                    response.ExpectedSchemas = GenerateExpectedSchemas(statusCodeGroup, context);

                    var nullableXmlAttribute = GetResponseXmlDocsElement(context.MethodInfo, httpStatusCode)?.Attribute("nullable");

                    var isResponseNullable = nullableXmlAttribute != null ?
                        nullableXmlAttribute.Value.ToLowerInvariant() == "true" :
                        statusCodeGroup.Any(r => r.IsNullable) &&
                            _settings.ReflectionService.GetDescription(contextualReturnType, _settings.DefaultResponseReferenceTypeNullHandling, _settings).IsNullable;

                    response.IsNullableRaw = isResponseNullable;
                    response.Schema = context.SchemaGenerator.GenerateWithReferenceAndNullability<JsonSchema>(
                        contextualReturnType, isResponseNullable, context.SchemaResolver);
                }

                context.OperationDescription.Operation.Responses[httpStatusCode] = response;
            }

            bool loadDefaultSuccessResponseFromReturnType;
            if (operationDescriptions.Any())
            {
                // If there are some attributes declared on the controller \ action, only return a default success response
                // if a 2xx status code isn't already defined and the SwaggerDefaultResponseAttribute is declared.
                var operationResponses = context.OperationDescription.Operation.Responses;
                var hasSuccessResponse = operationResponses.Keys.Any(HttpUtilities.IsSuccessStatusCode);

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

        private ICollection<JsonExpectedSchema> GenerateExpectedSchemas(
            IGrouping<string, OperationResponseDescription> group, OperationProcessorContext context)
        {
            if (group.Count() > 1)
            {
                var expectedSchemas = new List<JsonExpectedSchema>();
                foreach (var response in group)
                {
                    var contextualResponseType = response.ResponseType.ToContextualType();

                    var isNullable = _settings.ReflectionService.GetDescription(contextualResponseType, _settings).IsNullable;
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

            while (returnType.Name == "Task`1" || returnType.Name == "ActionResult`1")
            {
                returnType = returnType.GenericTypeArguments[0];
            }

            if (IsVoidResponse(returnType))
            {
                operation.Responses[GetVoidResponseStatusCode()] = new OpenApiResponse
                {
                    Description = successXmlDescription
                };
            }
            else
            {
                var returnParameterAttributes = returnParameter?.GetCustomAttributes(false)?.OfType<Attribute>() ?? Enumerable.Empty<Attribute>();
                var contextualReturnParameter = returnType.ToContextualType(returnParameterAttributes);

                var typeDescription = _settings.ReflectionService.GetDescription(contextualReturnParameter, _settings);
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

        private bool IsVoidResponse(Type returnType)
        {
            return returnType == null || returnType.FullName == "System.Void";
        }
    }
}
