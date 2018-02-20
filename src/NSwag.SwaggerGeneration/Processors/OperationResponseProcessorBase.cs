//-----------------------------------------------------------------------
// <copyright file="OperationResponseProcessorBase.cs" company="NSwag">
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
using NSwag.SwaggerGeneration.Processors.Contexts;

namespace NSwag.SwaggerGeneration.Processors
{
    /// <summary>The OperationResponseProcessor base class.</summary>
    public abstract class OperationResponseProcessorBase
    {
        private readonly JsonSchemaGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="OperationResponseProcessorBase"/> class.</summary>
        /// <param name="settings">The settings.</param>
        public OperationResponseProcessorBase(JsonSchemaGeneratorSettings settings)
        {
            _settings = settings;
        }

        /// <summary>Gets the response HTTP status code for an empty/void response and the given generator.</summary>
        /// <returns>The status code.</returns>
        protected abstract string GetVoidResponseStatusCode();

        /// <summary>Generates the responses based on the given return type attributes.</summary>
        /// <param name="operationProcessorContext">The context.</param>
        /// <param name="returnParameter">The return parameter.</param>
        /// <param name="responseTypeAttributes">The response type attributes.</param>
        /// <returns>The task.</returns>
        public async Task ProcessResponseTypeAttributes(OperationProcessorContext operationProcessorContext, ParameterInfo returnParameter, IEnumerable<Attribute> responseTypeAttributes)
        {
            var returnParameterAttributes = GetParameterAttributes(returnParameter);
            var successResponseDescription = await returnParameter.GetDescriptionAsync(returnParameterAttributes)
                .ConfigureAwait(false) ?? string.Empty;

            var responseDescriptions = GetOperationResponseDescriptions(responseTypeAttributes, successResponseDescription);
            await ProcessOperationDescriptionsAsync(responseDescriptions, returnParameter, operationProcessorContext, successResponseDescription);
        }
        
        /// <summary>Gets all attributes of the given parameter.</summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The attributes.</returns>
        protected IEnumerable<Attribute> GetParameterAttributes(ParameterInfo parameter)
        {
            try
            {
                return parameter?.GetCustomAttributes(true)?.Cast<Attribute>() ??
                    parameter?.GetCustomAttributes(false)?.Cast<Attribute>() ??
                    new Attribute[0];
            }
            catch
            {
                // in some environments, the call to GetCustomAttributes(true) fails
                return parameter?.GetCustomAttributes(false)?.Cast<Attribute>() ??
                    new Attribute[0];
            }
        }

        private IEnumerable<OperationResponseDescription> GetOperationResponseDescriptions(IEnumerable<Attribute> responseTypeAttributes, string successResponseDescription)
        {
            foreach (var attribute in responseTypeAttributes)
            {
                dynamic responseTypeAttribute = attribute;
                var attributeType = attribute.GetType();

                var isProducesAttributeWithNoType = // ignore ProducesAttribute if it has no type, https://github.com/RSuter/NSwag/issues/1201
                    attributeType.Name == "ProducesAttribute" && attribute.HasProperty("Type") && responseTypeAttribute.Type == null;

                if (!isProducesAttributeWithNoType)
                {
                    var returnType = typeof(void);
                    if (attributeType.GetRuntimeProperty("ResponseType") != null)
                        returnType = responseTypeAttribute.ResponseType;
                    else if (attributeType.GetRuntimeProperty("Type") != null)
                        returnType = responseTypeAttribute.Type;

                    if (returnType == null)
                        returnType = typeof(void);

                    var httpStatusCode = IsVoidResponse(returnType) ? GetVoidResponseStatusCode() : "200";
                    if (attributeType.GetRuntimeProperty("HttpStatusCode") != null && responseTypeAttribute.HttpStatusCode != null)
                        httpStatusCode = responseTypeAttribute.HttpStatusCode.ToString();
                    else if (attributeType.GetRuntimeProperty("StatusCode") != null && responseTypeAttribute.StatusCode != null)
                        httpStatusCode = responseTypeAttribute.StatusCode.ToString();

                    var description = HttpUtilities.IsSuccessStatusCode(httpStatusCode) ? successResponseDescription : string.Empty;
                    if (attributeType.GetRuntimeProperty("Description") != null)
                    {
                        if (!string.IsNullOrEmpty(responseTypeAttribute.Description))
                            description = responseTypeAttribute.Description;
                    }

                    var isNullable = true;
                    if (attributeType.GetRuntimeProperty("IsNullable") != null)
                        isNullable = responseTypeAttribute.IsNullable;

                    yield return new OperationResponseDescription(httpStatusCode, returnType, isNullable, description);
                }
            }
        }

        private async Task ProcessOperationDescriptionsAsync(IEnumerable<OperationResponseDescription> operationDescriptions, ParameterInfo returnParameter, OperationProcessorContext context, string successResponseDescription)
        {
            foreach (var statusCodeGroup in operationDescriptions.GroupBy(r => r.StatusCode))
            {
                var httpStatusCode = statusCodeGroup.Key;
                var returnType = statusCodeGroup.Select(r => r.ResponseType).FindCommonBaseType();
                var description = string.Join("\nor\n", statusCodeGroup.Select(r => r.Description));

                var typeDescription = _settings.ReflectionService.GetDescription(
                    returnType, GetParameterAttributes(returnParameter), _settings);

                var response = new SwaggerResponse
                {
                    Description = description ?? string.Empty
                };

                if (IsVoidResponse(returnType) == false)
                {
                    response.IsNullableRaw = statusCodeGroup.Any(r => r.IsNullable) && typeDescription.IsNullable;
                    response.ExpectedSchemas = await GenerateExpectedSchemasAsync(statusCodeGroup, context);
                    response.Schema = await context.SchemaGenerator
                        .GenerateWithReferenceAndNullability<JsonSchema4>(
                            returnType, null, typeDescription.IsNullable, context.SchemaResolver)
                        .ConfigureAwait(false);
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
                        .Any(a => a.GetType().IsAssignableTo("SwaggerDefaultResponseAttribute", TypeNameStyle.Name)) ||
                    context.MethodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes()
                        .Any(a => a.GetType().IsAssignableTo("SwaggerDefaultResponseAttribute", TypeNameStyle.Name));
            }
            else
            {
                // If there are no attributes declared on the controller \ action, always return a success response
                loadDefaultSuccessResponseFromReturnType = true;
            }

            if (loadDefaultSuccessResponseFromReturnType)
                await LoadDefaultSuccessResponseAsync(returnParameter, successResponseDescription, context);
        }

        private async Task<ICollection<JsonExpectedSchema>> GenerateExpectedSchemasAsync(
            IGrouping<string, OperationResponseDescription> group, OperationProcessorContext context)
        {
            if (group.Count() > 1)
            {
                var expectedSchemas = new List<JsonExpectedSchema>();
                foreach (var response in group)
                {
                    var isNullable = _settings.ReflectionService.GetDescription(response.ResponseType, null, _settings).IsNullable;
                    var schema = await context.SchemaGenerator.GenerateWithReferenceAndNullability<JsonSchema4>(
                        response.ResponseType, null, isNullable, context.SchemaResolver)
                        .ConfigureAwait(false);

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

        private async Task LoadDefaultSuccessResponseAsync(ParameterInfo returnParameter, string successXmlDescription, OperationProcessorContext context)
        {
            var operation = context.OperationDescription.Operation;
            var returnType = returnParameter.ParameterType;
            if (returnType == typeof(Task))
                returnType = typeof(void);
            else if (returnType.Name == "Task`1")
                returnType = returnType.GenericTypeArguments[0];

            if (IsVoidResponse(returnType))
            {
                operation.Responses[GetVoidResponseStatusCode()] = new SwaggerResponse
                {
                    Description = successXmlDescription
                };
            }
            else
            {
                var returnParameterAttributes = GetParameterAttributes(returnParameter);
                var typeDescription = _settings.ReflectionService.GetDescription(returnType, returnParameterAttributes, _settings);
                var responseSchema = await context.SchemaGenerator.GenerateWithReferenceAndNullability<JsonSchema4>(
                    returnType, returnParameterAttributes, typeDescription.IsNullable, context.SchemaResolver).ConfigureAwait(false);

                operation.Responses["200"] = new SwaggerResponse
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
