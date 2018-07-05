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
        private readonly JsonSchema4 _exceptionSchema;
        private readonly IClientGenerator _generator;
        private readonly CodeGeneratorSettingsBase _settings;
        private readonly bool _isPrimarySuccessResponse;

        /// <summary>Initializes a new instance of the <see cref="ResponseModelBase" /> class.</summary>
        /// <param name="operationModel">The operation model.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="response">The response.</param>
        /// <param name="isPrimarySuccessResponse">Specifies whether this is the success response.</param>
        /// <param name="exceptionSchema">The exception schema.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generator">The client generator.</param>
        protected ResponseModelBase(IOperationModel operationModel, 
            string statusCode, SwaggerResponse response, bool isPrimarySuccessResponse, 
            JsonSchema4 exceptionSchema, CodeGeneratorSettingsBase settings, IClientGenerator generator)
        {
            _response = response;
            _exceptionSchema = exceptionSchema;
            _generator = generator;
            _settings = settings;
            _isPrimarySuccessResponse = isPrimarySuccessResponse;
            _operationModel = operationModel;
            StatusCode = statusCode;
        }

        /// <summary>Gets the HTTP status code.</summary>
        public string StatusCode { get; }

        /// <summary>Gets a value indicating whether to check for the chunked HTTP status code (206, true when file response and 200/204).</summary>
        public bool CheckChunkedStatusCode => IsFile && (StatusCode == "200" || StatusCode == "204");

        /// <summary>Gets the type of the response.</summary>
        public string Type => _generator.GetTypeName(_response.ActualResponseSchema, IsNullable, "Response");

        /// <summary>Gets a value indicating whether the response has a type (i.e. not void).</summary>
        public bool HasType => Schema != null;

        /// <summary>Gets or sets the expected child schemas of the base schema (can be used for generating enhanced typings/documentation).</summary>
        public ICollection<JsonExpectedSchema> ExpectedSchemas => _response.ExpectedSchemas;

        /// <summary>Gets a value indicating whether the response is of type date.</summary>
        public bool IsDate {
            get
            {
                return _response.ActualResponseSchema != null && 
                      (_response.ActualResponseSchema.Format == JsonFormatStrings.Date ||
                       _response.ActualResponseSchema.Format == JsonFormatStrings.DateTime) &&
                       _generator.GetTypeName(_response.ActualResponseSchema, IsNullable, "Response") != "string";
            }
        }

        /// <summary>Gets a value indicating whether this is a file response.</summary>
        public bool IsFile => Schema?.ActualSchema.Type == JsonObjectType.File;

        /// <summary>Gets the response's exception description.</summary>
        public string ExceptionDescription => !string.IsNullOrEmpty(_response.Description) ?
            ConversionUtilities.ConvertToStringLiteral(_response.Description) :
            "A server side error occurred.";

        /// <summary>Gets the actual response schema.</summary>
        public JsonSchema4 ActualResponseSchema => _response.ActualResponseSchema;

        /// <summary>Gets the schema.</summary>
        private JsonSchema4 Schema => _response.ActualResponseSchema;

        /// <summary>Gets a value indicating whether the response is nullable.</summary>
        public bool IsNullable => _response.IsNullable(_settings.SchemaType);

        /// <summary>Gets a value indicating whether the response type inherits from exception.</summary>
        public bool InheritsExceptionSchema => _response.ActualResponseSchema?.InheritsSchema(_exceptionSchema) == true;

        /// <summary>Gets a value indicating whether this is the primary success response.</summary>
        public bool IsPrimarySuccessResponse => _isPrimarySuccessResponse;

        /// <summary>Gets a value indicating whether this is success response.</summary>
        public bool IsSuccess
        {
            get
            {
                if (_isPrimarySuccessResponse)
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
    }
}