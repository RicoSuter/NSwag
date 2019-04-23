//-----------------------------------------------------------------------
// <copyright file="ResponseModelBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration;

namespace NSwag.CodeGeneration.Models
{
    /// <summary>The response template model.</summary>
    public abstract class ResponseModelBase
    {
        private readonly IOperationModel _operationModel;
        private readonly SwaggerResponse _response;
        private readonly SwaggerOperation _operation;
        private readonly JsonSchema4 _exceptionSchema;
        private readonly IClientGenerator _generator;
        private readonly CodeGeneratorSettingsBase _settings;
        private readonly TypeResolverBase _resolver;

        /// <summary>Initializes a new instance of the <see cref="ResponseModelBase" /> class.</summary>
        /// <param name="operationModel">The operation model.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="response">The response.</param>
        /// <param name="isPrimarySuccessResponse">Specifies whether this is the success response.</param>
        /// <param name="exceptionSchema">The exception schema.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generator">The client generator.</param>
        protected ResponseModelBase(IOperationModel operationModel,
            SwaggerOperation operation,
            string statusCode, SwaggerResponse response, bool isPrimarySuccessResponse,
            JsonSchema4 exceptionSchema, TypeResolverBase resolver, CodeGeneratorSettingsBase settings, IClientGenerator generator)
        {
            _response = response;
            _operation = operation;
            _exceptionSchema = exceptionSchema;
            _generator = generator;
            _settings = settings;
            _resolver = resolver;
            _operationModel = operationModel;

            StatusCode = statusCode;
            IsPrimarySuccessResponse = isPrimarySuccessResponse;
            ActualResponseSchema = response.Schema?.ActualSchema;
        }

        /// <summary>Gets the HTTP status code.</summary>
        public string StatusCode { get; }

        /// <summary>Gets the actual response schema.</summary>
        public JsonSchema4 ActualResponseSchema { get; }

        /// <summary>Gets a value indicating whether to check for the chunked HTTP status code (206, true when file response and 200/204).</summary>
        public bool CheckChunkedStatusCode => IsFile && (StatusCode == "200" || StatusCode == "204");

        /// <summary>Gets the type of the response.</summary>
        public string Type => 
            _response.IsBinary(_operation) ? _generator.GetBinaryResponseTypeName() : 
            _generator.GetTypeName(ActualResponseSchema, IsNullable, "Response");

        /// <summary>Gets a value indicating whether the response has a type (i.e. not void).</summary>
        public bool HasType => ActualResponseSchema != null;

        /// <summary>Gets or sets the expected child schemas of the base schema (can be used for generating enhanced typings/documentation).</summary>
        public ICollection<JsonExpectedSchema> ExpectedSchemas => _response.ExpectedSchemas;

        /// <summary>Gets a value indicating whether the response is of type date.</summary>
        public bool IsDate
        {
            get
            {
                return ActualResponseSchema != null &&
                      (ActualResponseSchema.Format == JsonFormatStrings.Date ||
                       ActualResponseSchema.Format == JsonFormatStrings.DateTime) &&
                       _generator.GetTypeName(ActualResponseSchema, IsNullable, "Response") != "string";
            }
        }

        /// <summary>Gets a value indicating whether this is a text/plain response.</summary>
        public bool IsPlainText => _response.Content.ContainsKey("text/plain") || _operationModel.Produces == "text/plain";

        /// <summary>Gets a value indicating whether this is a file response.</summary>
        public bool IsFile => _response.IsBinary(_operation);

        /// <summary>Gets the response's exception description.</summary>
        public string ExceptionDescription => !string.IsNullOrEmpty(_response.Description) ?
            ConversionUtilities.ConvertToStringLiteral(_response.Description) :
            "A server side error occurred.";

        /// <summary>Gets the response schema.</summary>
        public JsonSchema4 ResolvableResponseSchema => _response.Schema != null ? _resolver.GetResolvableSchema(_response.Schema) : null;

        /// <summary>Gets a value indicating whether the response is nullable.</summary>
        public bool IsNullable => _response.IsNullable(_settings.SchemaType);

        /// <summary>Gets a value indicating whether the response type inherits from exception.</summary>
        public bool InheritsExceptionSchema => ActualResponseSchema?.InheritsSchema(_exceptionSchema) == true;

        /// <summary>Gets a value indicating whether this is the primary success response.</summary>
        public bool IsPrimarySuccessResponse { get; }

        /// <summary>Gets a value indicating whether this is success response.</summary>
        public bool IsSuccess
        {
            get
            {
                if (IsPrimarySuccessResponse)
                    return true;

                var primarySuccessResponse = _operationModel.Responses.FirstOrDefault(r => r.IsPrimarySuccessResponse);
                return HttpUtilities.IsSuccessStatusCode(StatusCode) && (
                    primarySuccessResponse == null ||
                    primarySuccessResponse.Type == Type
                );
            }
        }

        /// <summary>Gets a value indicating whether this is an exceptional response.</summary>
        public bool ThrowsException => !IsSuccess;

        /// <summary>Gets the response extension data.</summary>
        public IDictionary<string, object> ExtensionData => _response.ExtensionData;

        /// <summary>Gets the produced mime type of this response if available.</summary>
        public string Produces => _response.Content.Keys.FirstOrDefault();
    }
}