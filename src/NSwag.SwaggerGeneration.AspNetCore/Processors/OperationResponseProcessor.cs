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
using NJsonSchema.Infrastructure;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.Processors.Contexts;

namespace NSwag.SwaggerGeneration.AspNetCore.Processors
{
    /// <summary>Generates the operation's response objects based on reflection and the ResponseTypeAttribute, SwaggerResponseAttribute and ProducesResponseTypeAttribute attributes.</summary>
    public class OperationResponseProcessor : OperationResponseProcessorBase, IOperationProcessor
    {
        private readonly AspNetCoreToSwaggerGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="OperationParameterProcessor"/> class.</summary>
        /// <param name="settings">The settings.</param>
        public OperationResponseProcessor(AspNetCoreToSwaggerGeneratorSettings settings)
            : base(settings)
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

            var responseTypeAttributes = context.MethodInfo.GetCustomAttributes()
                .Where(a => a.GetType().Name == "ResponseTypeAttribute" ||
                            a.GetType().Name == "SwaggerResponseAttribute" ||
                            a.GetType().Name == "SwaggerDefaultResponseAttribute")
                .Concat(context.MethodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes()
                    .Where(a => a.GetType().Name == "SwaggerResponseAttribute" ||
                            a.GetType().Name == "SwaggerDefaultResponseAttribute"))
                .ToList();

            var operation = context.OperationDescription.Operation;
            foreach (var requestFormat in context.ApiDescription.SupportedRequestFormats)
            {
                if (operation.Consumes == null)
                    operation.Consumes = new List<string>();

                if (!operation.Consumes.Contains(requestFormat.MediaType, StringComparer.OrdinalIgnoreCase))
                {
                    operation.Consumes.Add(requestFormat.MediaType);
                }
            }

            if (responseTypeAttributes.Count > 0)
            {
                // if SwaggerResponseAttribute \ ResponseTypeAttributes are present, we'll only use those.
                await ProcessResponseTypeAttributes(context, parameter, responseTypeAttributes);
            }
            else
            {
                foreach (var apiResponse in context.ApiDescription.SupportedResponseTypes)
                {
                    var returnType = apiResponse.Type;
                    var response = new SwaggerResponse();
                    string httpStatusCode;
                    if (apiResponse.StatusCode == 0 && IsVoidResponse(returnType))
                        httpStatusCode = "200";
                    else if (apiResponse.TryGetPropertyValue<bool>("IsDefaultResponse"))
                        httpStatusCode = "default";
                    else
                        httpStatusCode = apiResponse.StatusCode.ToString(CultureInfo.InvariantCulture);

                    var typeDescription = _settings.ReflectionService.GetDescription(
                        returnType, GetParameterAttributes(context.MethodInfo.ReturnParameter), _settings);

                    if (IsVoidResponse(returnType) == false)
                    {
                        response.IsNullableRaw = typeDescription.IsNullable;

                        response.Schema = await context.SchemaGenerator
                            .GenerateWithReferenceAndNullability<JsonSchema4>(
                                returnType, null, typeDescription.IsNullable, context.SchemaResolver)
                            .ConfigureAwait(false);
                    }

                    context.OperationDescription.Operation.Responses[httpStatusCode] = response;

                    foreach (var responseFormat in apiResponse.ApiResponseFormats)
                    {
                        if (context.Document.Produces == null)
                            context.Document.Produces = new List<string>();

                        if (!context.Document.Produces.Contains(responseFormat.MediaType, StringComparer.OrdinalIgnoreCase))
                        {
                            context.Document.Produces.Add(responseFormat.MediaType);
                        }
                    }
                }
            }

            if (context.OperationDescription.Operation.Responses.Count == 0)
            {
                context.OperationDescription.Operation.Responses[GetVoidResponseStatusCode()] = new SwaggerResponse
                {
                    IsNullableRaw = true,
                    Schema = new JsonSchema4
                    {
                        Type = JsonObjectType.File
                    }
                };
            }

            var successXmlDescription = await parameter.GetDescriptionAsync(parameter.GetCustomAttributes())
                .ConfigureAwait(false) ?? string.Empty;

            foreach (var response in context.OperationDescription.Operation.Responses.Where(r =>
                HttpUtilities.IsSuccessStatusCode(r.Key)))
            {
                response.Value.Description = successXmlDescription;
            }

            return true;
        }

        /// <summary>Gets the response HTTP status code for an empty/void response and the given generator.</summary>
        /// <returns>The status code.</returns>
        protected override string GetVoidResponseStatusCode()
        {
            return "200";
        }

        private bool IsVoidResponse(Type returnType)
        {
            return returnType == null || returnType.FullName == "System.Void";
        }
    }
}