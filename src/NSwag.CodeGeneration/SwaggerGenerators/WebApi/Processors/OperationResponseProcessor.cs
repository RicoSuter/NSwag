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

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors
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
        /// <param name="document"></param>
        /// <param name="operationDescription">The operation description.</param>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="swaggerGenerator">The swagger generator.</param>
        /// <param name="allOperationDescriptions">All operation descriptions.</param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public bool Process(SwaggerService document, SwaggerOperationDescription operationDescription, MethodInfo methodInfo, 
            SwaggerGenerator swaggerGenerator, IList<SwaggerOperationDescription> allOperationDescriptions)
        {
            var successXmlDescription = methodInfo.ReturnParameter.GetXmlDocumentation() ?? string.Empty;

            var responseTypeAttributes = methodInfo.GetCustomAttributes()
                .Where(a => a.GetType().Name == "ResponseTypeAttribute" ||
                            a.GetType().Name == "SwaggerResponseAttribute")
                .ToList();

            var producesResponseTypeAttributes = methodInfo.GetCustomAttributes()
                .Where(a => a.GetType().Name == "ProducesResponseTypeAttribute")
                .ToList();

            if (responseTypeAttributes.Any() || producesResponseTypeAttributes.Any())
            {
                foreach (var attribute in responseTypeAttributes)
                {
                    dynamic responseTypeAttribute = attribute;
                    var attributeType = attribute.GetType();

                    var returnType = typeof(void);
                    if (attributeType.GetRuntimeProperty("ResponseType") != null)
                        returnType = responseTypeAttribute.ResponseType;
                    else if (attributeType.GetRuntimeProperty("Type") != null)
                        returnType = responseTypeAttribute.Type;

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

                    var typeDescription = JsonObjectTypeDescription.FromType(returnType, methodInfo.ReturnParameter?.GetCustomAttributes(), _settings.DefaultEnumHandling);
                    var response = new SwaggerResponse
                    {
                        Description = description ?? string.Empty
                    };

                    if (IsVoidResponse(returnType) == false)
                    {
                        response.IsNullableRaw = typeDescription.IsNullable;
                        response.Schema = swaggerGenerator.GenerateAndAppendSchemaFromType(returnType, typeDescription.IsNullable, null);
                    }

                    operationDescription.Operation.Responses[httpStatusCode] = response;
                }

                foreach (dynamic producesResponseTypeAttribute in producesResponseTypeAttributes)
                {
                    var returnType = producesResponseTypeAttribute.Type;
                    var typeDescription = JsonObjectTypeDescription.FromType(returnType, methodInfo.ReturnParameter?.GetCustomAttributes(), _settings.DefaultEnumHandling);

                    var httpStatusCode = producesResponseTypeAttribute.StatusCode.ToString(CultureInfo.InvariantCulture);
                    var response = new SwaggerResponse
                    {
                        Description = HttpUtilities.IsSuccessStatusCode(httpStatusCode) ? successXmlDescription : string.Empty
                    };

                    if (IsVoidResponse(returnType) == false)
                    {
                        response.IsNullableRaw = typeDescription.IsNullable;
                        response.Schema = swaggerGenerator.GenerateAndAppendSchemaFromType(returnType, typeDescription.IsNullable, null);
                    }

                    operationDescription.Operation.Responses[httpStatusCode] = response;
                }
            }
            else
                LoadDefaultSuccessResponse(operationDescription.Operation, methodInfo, successXmlDescription, swaggerGenerator);

            return true;
        }
        
        private void LoadDefaultSuccessResponse(SwaggerOperation operation, MethodInfo methodInfo, string responseDescription, SwaggerGenerator swaggerGenerator)
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
                var typeDescription = JsonObjectTypeDescription.FromType(returnType, 
                    methodInfo.ReturnParameter?.GetCustomAttributes(), _settings.DefaultEnumHandling);

                operation.Responses["200"] = new SwaggerResponse
                {
                    Description = responseDescription,
                    IsNullableRaw = typeDescription.IsNullable,
                    Schema = swaggerGenerator.GenerateAndAppendSchemaFromType(returnType, typeDescription.IsNullable, null)
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