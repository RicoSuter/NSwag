//-----------------------------------------------------------------------
// <copyright file="ResponseModelBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;
using NJsonSchema.CodeGeneration;

namespace NSwag.CodeGeneration.Models
{
    /// <summary>The response template model.</summary>
    public abstract class ResponseModelBase
    {
        private readonly SwaggerResponse _response;
        private readonly JsonSchema4 _exceptionSchema;
        private readonly IClientGenerator _generator;
        private readonly CodeGeneratorSettingsBase _settings;

        /// <summary>Initializes a new instance of the <see cref="ResponseModelBase" /> class.</summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="response">The response.</param>
        /// <param name="isSuccessResponse">Specifies whether this is the success response.</param>
        /// <param name="exceptionSchema">The exception schema.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="generator">The client generator.</param>
        protected ResponseModelBase(string statusCode, SwaggerResponse response, bool isSuccessResponse, JsonSchema4 exceptionSchema, CodeGeneratorSettingsBase settings, IClientGenerator generator)
        {
            _response = response;
            _exceptionSchema = exceptionSchema;
            _generator = generator;
            _settings = settings;

            IsSuccess = isSuccessResponse;
            StatusCode = statusCode;
        }

        /// <summary>Gets the HTTP status code.</summary>
        public string StatusCode { get; }

        /// <summary>Gets the type of the response.</summary>
        public string Type => _generator.GetTypeName(_response.ActualResponseSchema, IsNullable, "Response");

        /// <summary>Gets a value indicating whether the response has a type (i.e. not void).</summary>
        public bool HasType => Schema != null;

        /// <summary>Gets a value indicating whether this is success response.</summary>
        public bool IsSuccess { get; }

        /// <summary>Gets a value indicating whether the response is of type date.</summary>
        public bool IsDate =>
            (_response.ActualResponseSchema.Format == JsonFormatStrings.DateTime ||
            _response.ActualResponseSchema.Format == JsonFormatStrings.Date) &&
            _generator.GetTypeName(_response.ActualResponseSchema, IsNullable, "Response") != "string";

        /// <summary>Gets a value indicating whether this is a file response.</summary>
        public bool IsFile => Schema != null && Schema.ActualSchema.Type == JsonObjectType.File;

        /// <summary>Gets the response's exception description.</summary>
        public string ExceptionDescription => !string.IsNullOrEmpty(_response.Description) ?
            ConversionUtilities.ConvertToStringLiteral(_response.Description) :
            "A server side error occurred.";

        /// <summary>Gets the actual response schema.</summary>
        public JsonSchema4 ActualResponseSchema => _response.ActualResponseSchema;

        /// <summary>Gets the schema.</summary>
        private JsonSchema4 Schema => _response.ActualResponseSchema;

        /// <summary>Gets a value indicating whether the response is nullable.</summary>
        public bool IsNullable => _response.IsNullable(_settings.NullHandling);

        /// <summary>Gets a value indicating whether the response type inherits from exception.</summary>
        public bool InheritsExceptionSchema => _response.InheritsExceptionSchema(_exceptionSchema);
    }
}