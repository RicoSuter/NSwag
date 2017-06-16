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
using NJsonSchema.Generation;
using NJsonSchema.Infrastructure;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.Processors.Contexts;
using NSwag.SwaggerGeneration.WebApi.Processors.Models;

namespace NSwag.SwaggerGeneration.WebApi.Processors
{
    /// <summary>Generates the operation's response objects based on reflection and the ResponseTypeAttribute, SwaggerResponseAttribute and ProducesResponseTypeAttribute attributes.</summary>
    public class OperationResponseProcessor : IOperationProcessor
    {
        private readonly WebApiToSwaggerGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="OperationParameterProcessor"/> class.</summary>
        /// <param name="settings">The settings.</param>
        public OperationResponseProcessor(WebApiToSwaggerGeneratorSettings settings)
        {
            _settings = settings;
        }

        /// <summary>Processes the specified method information.</summary>
        /// <param name="context"></param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public async Task<bool> ProcessAsync(OperationProcessorContext context)
        {
            var parameter = context.MethodInfo.ReturnParameter;
            var successXmlDescription = await parameter.GetDescriptionAsync(parameter.GetCustomAttributes()).ConfigureAwait(false) ?? string.Empty;

            var responseTypeAttributes = context.MethodInfo.GetCustomAttributes()
                .Where(a => a.GetType().Name == "ResponseTypeAttribute" ||
                            a.GetType().Name == "SwaggerResponseAttribute")
                .Concat(context.MethodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes()
                    .Where(a => a.GetType().Name == "SwaggerResponseAttribute"))
                .ToList();

            var producesResponseTypeAttributes = context.MethodInfo.GetCustomAttributes()
                .Where(a => a.GetType().Name == "ProducesResponseTypeAttribute")
                .ToList();

            if (responseTypeAttributes.Any() || producesResponseTypeAttributes.Any())
            {
                var responses = new List<OperationResponseModel>();
                foreach (var attribute in responseTypeAttributes)
                {
                    dynamic responseTypeAttribute = attribute;
                    var attributeType = attribute.GetType();

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

                    var description = HttpUtilities.IsSuccessStatusCode(httpStatusCode) ? successXmlDescription : string.Empty;
                    if (attributeType.GetRuntimeProperty("Description") != null)
                    {
                        if (!string.IsNullOrEmpty(responseTypeAttribute.Description))
                            description = responseTypeAttribute.Description;
                    }

                    responses.Add(new OperationResponseModel(httpStatusCode, returnType, description));
                }

                foreach (dynamic producesResponseTypeAttribute in producesResponseTypeAttributes)
                {
                    var returnType = producesResponseTypeAttribute.Type;
                    var httpStatusCode = producesResponseTypeAttribute.StatusCode.ToString(CultureInfo.InvariantCulture);
                    var description = HttpUtilities.IsSuccessStatusCode(httpStatusCode) ? successXmlDescription : string.Empty;
                    responses.Add(new OperationResponseModel(httpStatusCode, returnType, description));
                }

                foreach (var group in responses.GroupBy(r => r.HttpStatusCode))
                {
                    var httpStatusCode = group.Key;
                    var returnType = group.Select(r => r.ResponseType).FindCommonBaseType();
                    var description = string.Join("\nor\n", group.Select(r => r.Description));

                    var typeDescription = JsonObjectTypeDescription.FromType(returnType, _settings.ResolveContract(returnType), context.MethodInfo.ReturnParameter?.GetCustomAttributes(), _settings.DefaultEnumHandling);
                    var response = new SwaggerResponse
                    {
                        Description = description ?? string.Empty
                    };

                    if (IsVoidResponse(returnType) == false)
                    {
                        response.IsNullableRaw = typeDescription.IsNullable;
                        response.Schema = await context.SwaggerGenerator.GenerateAndAppendSchemaFromTypeAsync(returnType, typeDescription.IsNullable, null).ConfigureAwait(false);
                        response.ExpectedSchemas = await GenerateExpectedSchemasAsync(context, group);
                    }

                    context.OperationDescription.Operation.Responses[httpStatusCode] = response;
                }

                var loadSuccessResponseFromReturnType = context.MethodInfo.GetCustomAttributes()
                        .Any(a => a.GetType().IsAssignableTo("SwaggerDefaultResponseAttribute", TypeNameStyle.Name)) ||
                    context.MethodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes()
                        .Any(a => a.GetType().IsAssignableTo("SwaggerDefaultResponseAttribute", TypeNameStyle.Name));

                if (loadSuccessResponseFromReturnType)
                    await LoadDefaultSuccessResponseAsync(context.OperationDescription.Operation, context.MethodInfo, successXmlDescription, context.SwaggerGenerator).ConfigureAwait(false);
            }
            else
                await LoadDefaultSuccessResponseAsync(context.OperationDescription.Operation, context.MethodInfo, successXmlDescription, context.SwaggerGenerator).ConfigureAwait(false);

            return true;
        }

        private async Task<ICollection<JsonExpectedSchema>> GenerateExpectedSchemasAsync(OperationProcessorContext context, IGrouping<string, OperationResponseModel> group)
        {
            if (group.Count() > 1)
            {
                var expectedSchemas = new List<JsonExpectedSchema>();
                foreach (var response in group)
                {
                    var isNullable = JsonObjectTypeDescription.FromType(response.ResponseType, _settings.ResolveContract(response.ResponseType), null, _settings.DefaultEnumHandling).IsNullable;
                    var schema = await context.SwaggerGenerator.GenerateAndAppendSchemaFromTypeAsync(response.ResponseType, isNullable, null).ConfigureAwait(false);
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

        private async Task LoadDefaultSuccessResponseAsync(SwaggerOperation operation, MethodInfo methodInfo, string responseDescription, SwaggerGenerator swaggerGenerator)
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

                var typeDescription = JsonObjectTypeDescription.FromType(returnType, _settings.ResolveContract(returnType), attributes, _settings.DefaultEnumHandling);
                operation.Responses["200"] = new SwaggerResponse
                {
                    Description = responseDescription,
                    IsNullableRaw = typeDescription.IsNullable,
                    Schema = await swaggerGenerator.GenerateAndAppendSchemaFromTypeAsync(returnType, typeDescription.IsNullable, null).ConfigureAwait(false)
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