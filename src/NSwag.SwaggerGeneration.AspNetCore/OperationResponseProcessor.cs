//-----------------------------------------------------------------------
// <copyright file="OperationResponseProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Infrastructure;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.Processors.Contexts;
using NSwag.SwaggerGeneration.WebApi.Processors.Models;

namespace NSwag.SwaggerGeneration.AspNetCore
{
    /// <summary>Generates the operation's response objects based on reflection and the ResponseTypeAttribute, SwaggerResponseAttribute and ProducesResponseTypeAttribute attributes.</summary>
    public class OperationResponseProcessor : IOperationProcessor
    {
        private readonly AspNetCoreToSwaggerGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="OperationParameterProcessor"/> class.</summary>
        /// <param name="settings">The settings.</param>
        public OperationResponseProcessor(AspNetCoreToSwaggerGeneratorSettings settings)
        {
            _settings = settings;
        }

        /// <summary>Processes the specified method information.</summary>
        /// <param name="operationProcessorContext"></param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public async Task<bool> ProcessAsync(OperationProcessorContext operationProcessorContext)
        {
            if (!(operationProcessorContext is AspNetCoreOperationProcessorContext context))
                return false;

            var parameter = context.MethodInfo.ReturnParameter;
            var successXmlDescription = await parameter.GetDescriptionAsync(parameter.GetCustomAttributes()).ConfigureAwait(false) ?? string.Empty;

            var responseTypeAttributes = context.MethodInfo.GetCustomAttributes()
                .Where(a => a.GetType().Name == "ResponseTypeAttribute" ||
                            a.GetType().Name == "SwaggerResponseAttribute")
                .Concat(context.MethodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes()
                    .Where(a => a.GetType().Name == "SwaggerResponseAttribute"))
                .ToList();

            if (context.ApiDescription.SupportedResponseTypes.Any())
            {
                var responses = new List<OperationResponseModel>();
                foreach (var apiResponse in context.ApiDescription.SupportedResponseTypes)
                {
                    var returnType = apiResponse.Type;
                    // Attempt to look up a Swagger attribute by type.
                    var attribute = responseTypeAttributes.FirstOrDefault(a =>
                    {
                        if (a.GetType().GetRuntimeProperty("ResponseType") != null)
                            return ((dynamic)a).ResponseType == returnType;
                        else if (a.GetType().GetRuntimeProperty("Type") != null)
                            return ((dynamic)a).Type == returnType;
                        else
                            return false;
                    });

                    var attributeType = attribute?.GetType();
                    dynamic responseTypeAttribute = attribute;

                    string httpStatusCode;
                    if (apiResponse.StatusCode == 0 && IsVoidResponse(returnType))
                        httpStatusCode = GetVoidResponseStatusCode();
                    else if (apiResponse.TryGetPropertyValue<bool>("IsDefaultResponse"))
                        httpStatusCode = "default";
                    else
                        httpStatusCode = apiResponse.StatusCode.ToString(CultureInfo.InvariantCulture);

                    if (attributeType?.GetRuntimeProperty("HttpStatusCode") != null && responseTypeAttribute.HttpStatusCode != null)
                        httpStatusCode = responseTypeAttribute.HttpStatusCode.ToString();
                    else if (attributeType?.GetRuntimeProperty("StatusCode") != null && responseTypeAttribute.StatusCode != null)
                        httpStatusCode = responseTypeAttribute.StatusCode.ToString();

                    var description = HttpUtilities.IsSuccessStatusCode(httpStatusCode) ? successXmlDescription : string.Empty;
                    if (attributeType?.GetRuntimeProperty("Description") != null)
                    {
                        if (!string.IsNullOrEmpty(responseTypeAttribute.Description))
                            description = responseTypeAttribute.Description;
                    }

                    var isNullable = true;
                    if (attributeType?.GetRuntimeProperty("IsNullable") != null)
                        isNullable = responseTypeAttribute.IsNullable;

                    responses.Add(new OperationResponseModel(httpStatusCode, returnType, isNullable, description));
                }

                foreach (var statusCodeGroup in responses.GroupBy(r => r.HttpStatusCode))
                {
                    var httpStatusCode = statusCodeGroup.Key;
                    var returnType = statusCodeGroup.Select(r => r.ResponseType).FindCommonBaseType();
                    var description = string.Join("\nor\n", statusCodeGroup.Select(r => r.Description));

                    var typeDescription = _settings.ReflectionService.GetDescription(
                        returnType, context.MethodInfo.ReturnParameter?.GetCustomAttributes(), _settings);
                    var response = new SwaggerResponse
                    {
                        Description = description ?? string.Empty
                    };

                    if (IsVoidResponse(returnType) == false)
                    {
                        response.IsNullableRaw = responses.Any(r => r.IsNullable) && typeDescription.IsNullable;
                        response.ExpectedSchemas = await GenerateExpectedSchemasAsync(context, statusCodeGroup);
                        response.Schema = await context.SchemaGenerator
                            .GenerateWithReferenceAndNullability<JsonSchema4>(
                                returnType, null, typeDescription.IsNullable, context.SchemaResolver)
                            .ConfigureAwait(false);
                    }

                    context.OperationDescription.Operation.Responses[httpStatusCode] = response;
                }

                var operationResponses = context.OperationDescription.Operation.Responses;
                var hasSuccessResponse = operationResponses.ContainsKey("200") || operationResponses.ContainsKey("204");

                var loadDefaultSuccessResponseFromReturnType = !hasSuccessResponse &&
                    context.MethodInfo.GetCustomAttributes()
                        .Any(a => a.GetType().IsAssignableTo("SwaggerDefaultResponseAttribute", TypeNameStyle.Name)) ||
                    context.MethodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes()
                        .Any(a => a.GetType().IsAssignableTo("SwaggerDefaultResponseAttribute", TypeNameStyle.Name));

                if (loadDefaultSuccessResponseFromReturnType)
                    await LoadDefaultSuccessResponseAsync(context.OperationDescription.Operation, context.MethodInfo,
                        successXmlDescription, context.SchemaGenerator, context.SchemaResolver).ConfigureAwait(false);
            }
            else
                await LoadDefaultSuccessResponseAsync(context.OperationDescription.Operation, context.MethodInfo,
                    successXmlDescription, context.SchemaGenerator, context.SchemaResolver).ConfigureAwait(false);

            return true;
        }

        private async Task<ICollection<JsonExpectedSchema>> GenerateExpectedSchemasAsync(OperationProcessorContext context, IGrouping<string, OperationResponseModel> group)
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

        private async Task LoadDefaultSuccessResponseAsync(
            SwaggerOperation operation, MethodInfo methodInfo, string responseDescription,
            JsonSchemaGenerator schemaGenerator, JsonSchemaResolver schemaResolver)
        {
            var returnType = methodInfo.ReturnType;
            if (returnType == typeof(Task))
                returnType = typeof(void);
            else if (returnType.Name == "Task`1")
                returnType = returnType.GenericTypeArguments[0];

            if (IsVoidResponse(returnType))
            {
                operation.Responses[GetVoidResponseStatusCode()] = new SwaggerResponse
                {
                    Description = responseDescription
                };
            }
            else
            {
                IEnumerable<Attribute> attributes;
                try
                {
                    attributes = methodInfo.ReturnParameter?.GetCustomAttributes(true).OfType<Attribute>();
                }
                catch
                {
                    attributes = methodInfo.ReturnParameter?.GetCustomAttributes(false).OfType<Attribute>();
                }

                var typeDescription = _settings.ReflectionService.GetDescription(returnType, attributes, _settings);
                operation.Responses["200"] = new SwaggerResponse
                {
                    Description = responseDescription,
                    IsNullableRaw = typeDescription.IsNullable,
                    Schema = await schemaGenerator.GenerateWithReferenceAndNullability<JsonSchema4>(
                        returnType, attributes, typeDescription.IsNullable, schemaResolver)
                        .ConfigureAwait(false)
                };
            }
        }

        private bool IsVoidResponse(Type returnType)
        {
            return returnType == null || returnType.FullName == "System.Void";
        }

        private string GetVoidResponseStatusCode()
        {
            return _settings.IsAspNetCore ? "200" : "204";
        }
    }
}