//-----------------------------------------------------------------------
// <copyright file="AspNetCoreToSwaggerGenerator.cs" company="NSwag">
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
using NSwag.SwaggerGeneration.Processors.Contexts;
using NSwag.SwaggerGeneration.Processors;

namespace NSwag.SwaggerGeneration.WebApi.Processors
{
    public class SwaggerResponseBuilder
    {
        private readonly OperationProcessorContext _context;
        private readonly JsonSchemaGeneratorSettings _settings;
        private readonly string _voidResponseStatusCode;
        private readonly string _successXmlDescription;

        public SwaggerResponseBuilder(
            OperationProcessorContext context,
            JsonSchemaGeneratorSettings settings,
            string voidResponseStatusCode,
            string successXmlDescription)
        {
            _context = context;
            _settings = settings;
            _voidResponseStatusCode = voidResponseStatusCode;
            _successXmlDescription = successXmlDescription;
        }

        public List<OperationResponseDescription> OperationResponseModels { get; } = new List<OperationResponseDescription>();

        public void PopulateModelsFromResponseTypeAttributes(IEnumerable<Attribute> responseTypeAttributes)
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

                if (returnType == null)
                    returnType = typeof(void);

                var httpStatusCode = IsVoidResponse(returnType) ? _voidResponseStatusCode : "200";
                if (attributeType.GetRuntimeProperty("HttpStatusCode") != null && responseTypeAttribute.HttpStatusCode != null)
                    httpStatusCode = responseTypeAttribute.HttpStatusCode.ToString();
                else if (attributeType.GetRuntimeProperty("StatusCode") != null && responseTypeAttribute.StatusCode != null)
                    httpStatusCode = responseTypeAttribute.StatusCode.ToString();

                var description = HttpUtilities.IsSuccessStatusCode(httpStatusCode) ? _successXmlDescription : string.Empty;
                if (attributeType.GetRuntimeProperty("Description") != null)
                {
                    if (!string.IsNullOrEmpty(responseTypeAttribute.Description))
                        description = responseTypeAttribute.Description;
                }

                var isNullable = true;
                if (attributeType.GetRuntimeProperty("IsNullable") != null)
                    isNullable = responseTypeAttribute.IsNullable;

                OperationResponseModels.Add(new OperationResponseDescription(httpStatusCode, returnType, isNullable, description));
            }
        }

        public void PopulateModelsFromProducesAttributes(IEnumerable<Attribute> producesResponseTypeAttributes)
        {
            foreach (dynamic producesResponseTypeAttribute in producesResponseTypeAttributes)
            {
                var returnType = producesResponseTypeAttribute.Type;
                var httpStatusCode = producesResponseTypeAttribute.StatusCode.ToString(CultureInfo.InvariantCulture);
                var description = HttpUtilities.IsSuccessStatusCode(httpStatusCode) ? _successXmlDescription : string.Empty;
                OperationResponseModels.Add(new OperationResponseDescription(httpStatusCode, returnType, true, description));
            }
        }
        
        public async Task BuildSwaggerResponseAsync(ParameterInfo returnParameter)
        {
            foreach (var statusCodeGroup in OperationResponseModels.GroupBy(r => r.StatusCode))
            {
                var httpStatusCode = statusCodeGroup.Key;
                var returnType = statusCodeGroup.Select(r => r.ResponseType).FindCommonBaseType();
                var description = string.Join("\nor\n", statusCodeGroup.Select(r => r.Description));

                var typeDescription = _settings.ReflectionService.GetDescription(
                    returnType, returnParameter?.GetCustomAttributes(), _settings);

                var response = new SwaggerResponse
                {
                    Description = description ?? string.Empty
                };

                if (IsVoidResponse(returnType) == false)
                {
                    response.IsNullableRaw = OperationResponseModels.Any(r => r.IsNullable) && typeDescription.IsNullable;
                    response.ExpectedSchemas = await GenerateExpectedSchemasAsync(statusCodeGroup);
                    response.Schema = await _context.SchemaGenerator
                        .GenerateWithReferenceAndNullability<JsonSchema4>(
                            returnType, null, typeDescription.IsNullable, _context.SchemaResolver)
                        .ConfigureAwait(false);
                }

                _context.OperationDescription.Operation.Responses[httpStatusCode] = response;
            }

            bool loadDefaultSuccessResponseFromReturnType;
            if (OperationResponseModels.Count > 0)
            {
                // If there are some attributes declared on the controller \ action, only return a default success response
                // if a 2xx status code isn't already defined and the SwaggerDefaultResponseAttribute is declared.
                var operationResponses = _context.OperationDescription.Operation.Responses;
                var hasSuccessResponse = operationResponses.Keys.Any(HttpUtilities.IsSuccessStatusCode);

                loadDefaultSuccessResponseFromReturnType = !hasSuccessResponse &&
                    _context.MethodInfo.GetCustomAttributes()
                        .Any(a => a.GetType().IsAssignableTo("SwaggerDefaultResponseAttribute", TypeNameStyle.Name)) ||
                    _context.MethodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes()
                        .Any(a => a.GetType().IsAssignableTo("SwaggerDefaultResponseAttribute", TypeNameStyle.Name));
            }
            else
            {
                // If there are no attributes declared on the controller \ action, always return a success response
                loadDefaultSuccessResponseFromReturnType = true;
            }

            if (loadDefaultSuccessResponseFromReturnType)
                await LoadDefaultSuccessResponseAsync(returnParameter);
        }

        private async Task<ICollection<JsonExpectedSchema>> GenerateExpectedSchemasAsync(
            IGrouping<string, OperationResponseDescription> group)
        {
            if (group.Count() > 1)
            {
                var expectedSchemas = new List<JsonExpectedSchema>();
                foreach (var response in group)
                {
                    var isNullable = _settings.ReflectionService.GetDescription(response.ResponseType, null, _settings).IsNullable;
                    var schema = await _context.SchemaGenerator.GenerateWithReferenceAndNullability<JsonSchema4>(
                        response.ResponseType, null, isNullable, _context.SchemaResolver)
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

        private async Task LoadDefaultSuccessResponseAsync(ParameterInfo returnParameter)
        {
            var operation = _context.OperationDescription.Operation;
            var returnType = returnParameter.ParameterType;
            if (returnType == typeof(Task))
                returnType = typeof(void);
            else if (returnType.Name == "Task`1")
                returnType = returnType.GenericTypeArguments[0];

            if (IsVoidResponse(returnType))
            {
                operation.Responses[_voidResponseStatusCode] = new SwaggerResponse
                {
                    Description = _successXmlDescription
                };
            }
            else
            {
                IEnumerable<Attribute> attributes;
                try
                {
                    attributes = returnParameter?.GetCustomAttributes(true).Cast<Attribute>();
                }
                catch
                {
                    attributes = returnParameter?.GetCustomAttributes(false).Cast<Attribute>();
                }

                var typeDescription = _settings.ReflectionService.GetDescription(returnType, attributes, _settings);
                operation.Responses["200"] = new SwaggerResponse
                {
                    Description = _successXmlDescription,
                    IsNullableRaw = typeDescription.IsNullable,
                    Schema = await _context.SchemaGenerator.GenerateWithReferenceAndNullability<JsonSchema4>(
                        returnType, attributes, typeDescription.IsNullable, _context.SchemaResolver)
                        .ConfigureAwait(false)
                };
            }
        }

        private static bool IsVoidResponse(Type returnType)
        {
            return returnType == null || returnType.FullName == "System.Void";
        }
    }
}
