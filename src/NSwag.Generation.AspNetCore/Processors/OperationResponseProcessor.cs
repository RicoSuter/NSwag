//-----------------------------------------------------------------------
// <copyright file="OperationResponseProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Globalization;
using System.Reflection;
using Namotion.Reflection;
using NJsonSchema;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace NSwag.Generation.AspNetCore.Processors
{
    /// <summary>Generates the operation's response objects based on reflection and the ResponseTypeAttribute, SwaggerResponseAttribute and ProducesResponseTypeAttribute attributes.</summary>
    public class OperationResponseProcessor : OperationResponseProcessorBase, IOperationProcessor
    {
        private readonly AspNetCoreOpenApiDocumentGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="OperationParameterProcessor"/> class.</summary>
        /// <param name="settings">The settings.</param>
        public OperationResponseProcessor(AspNetCoreOpenApiDocumentGeneratorSettings settings)
            : base(settings)
        {
            _settings = settings;
        }

        /// <summary>Processes the specified method information.</summary>
        /// <param name="operationProcessorContext"></param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public bool Process(OperationProcessorContext operationProcessorContext)
        {
            if (operationProcessorContext is not AspNetCoreOperationProcessorContext context)
            {
                return false;
            }

            var responseTypeAttributes = context.MethodInfo?
                .GetCustomAttributes()
                .Where(a => a.GetType().IsAssignableToTypeName("ResponseTypeAttribute", TypeNameStyle.Name) ||
                            a.GetType().IsAssignableToTypeName("SwaggerResponseAttribute", TypeNameStyle.Name) ||
                            a.GetType().IsAssignableToTypeName("SwaggerDefaultResponseAttribute", TypeNameStyle.Name))
                .Concat(context.MethodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes()
                    .Where(a => a.GetType().IsAssignableToTypeName("SwaggerResponseAttribute", TypeNameStyle.Name) ||
                                a.GetType().IsAssignableToTypeName("SwaggerDefaultResponseAttribute", TypeNameStyle.Name)))
                .ToArray() ?? [];

            if (responseTypeAttributes.Length > 0)
            {
                // if SwaggerResponseAttribute \ ResponseTypeAttributes are present, we'll only use those.
                ProcessResponseTypeAttributes(context, responseTypeAttributes);
            }
            else
            {
                foreach (var apiResponse in context.ApiDescription.SupportedResponseTypes)
                {
                    var returnType = apiResponse.Type;
                    var response = new OpenApiResponse();
                    string httpStatusCode;

                    if (apiResponse.TryGetPropertyValue<bool>("IsDefaultResponse"))
                    {
                        httpStatusCode = "default";
                    }
                    else if (apiResponse.StatusCode == 0 && IsVoidResponse(returnType))
                    {
                        httpStatusCode = "200";
                    }
                    else
                    {
                        httpStatusCode = apiResponse.StatusCode.ToString(CultureInfo.InvariantCulture);
                    }

                    if (!IsVoidResponse(returnType))
                    {
                        var returnTypeAttributes = context.MethodInfo?.ReturnParameter?.GetCustomAttributes(false).OfType<Attribute>() ?? [];
                        var contextualReturnType = returnType.ToContextualType(returnTypeAttributes);

                        var nullableXmlAttribute = GetResponseXmlDocsElement(context.MethodInfo, httpStatusCode)?.Attribute("nullable");
                        var isResponseNullable = nullableXmlAttribute != null ?
                                                 nullableXmlAttribute.Value.Equals("true", StringComparison.OrdinalIgnoreCase) :
                                                 _settings.SchemaSettings.ReflectionService.GetDescription(contextualReturnType, _settings.DefaultResponseReferenceTypeNullHandling, _settings.SchemaSettings).IsNullable;

                        response.IsNullableRaw = isResponseNullable;
                        response.Schema = context.SchemaGenerator.GenerateWithReferenceAndNullability<JsonSchema>(
                            contextualReturnType, isResponseNullable, context.SchemaResolver);
                    }

                    context.OperationDescription.Operation.Responses[httpStatusCode] = response;
                }
            }

            if (context.OperationDescription.Operation.Responses.Count == 0)
            {
                context.OperationDescription.Operation.Responses[GetVoidResponseStatusCode()] = new OpenApiResponse
                {
                    IsNullableRaw = true,
                    Schema = new JsonSchema
                    {
                        Type = _settings.SchemaSettings.SchemaType == SchemaType.Swagger2 ? JsonObjectType.File : JsonObjectType.String,
                        Format = _settings.SchemaSettings.SchemaType == SchemaType.Swagger2 ? null : JsonFormatStrings.Binary,
                    }
                };
            }

            UpdateResponseDescription(context);
            return true;
        }

        /// <summary>Gets the response HTTP status code for an empty/void response and the given generator.</summary>
        /// <returns>The status code.</returns>
        protected override string GetVoidResponseStatusCode()
        {
            return "200";
        }

        private static bool IsVoidResponse(Type returnType)
        {
            return returnType == null || returnType.FullName == "System.Void";
        }
    }
}
