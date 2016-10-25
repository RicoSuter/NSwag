//-----------------------------------------------------------------------
// <copyright file="ResponseModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using NJsonSchema;
using NJsonSchema.CodeGeneration;

namespace NSwag.CodeGeneration.CodeGenerators.Models
{
    /// <summary>The response template model.</summary>
    public class ResponseModel
    {
        private readonly SwaggerResponse _response;
        private readonly JsonSchema4 _exceptionSchema;
        private readonly ClientGeneratorBase _clientGeneratorBase;

        /// <summary>Initializes a new instance of the <see cref="ResponseModel" /> class.</summary>
        /// <param name="response">The response.</param>
        /// <param name="exceptionSchema">The exception schema.</param>
        /// <param name="clientGeneratorBase">The client generator base.</param>
        public ResponseModel(KeyValuePair<string, SwaggerResponse> response, JsonSchema4 exceptionSchema, ClientGeneratorBase clientGeneratorBase)
        {
            _response = response.Value;
            _exceptionSchema = exceptionSchema;
            _clientGeneratorBase = clientGeneratorBase;

            StatusCode = response.Key;
        }

        /// <summary>Gets the HTTP status code.</summary>
        public string StatusCode { get; }

        /// <summary>Gets the type of the response.</summary>
        public string Type => _clientGeneratorBase.GetType(_response.ActualResponseSchema, IsNullable, "Response");

        /// <summary>Gets a value indicating whether the response has a type (i.e. not void).</summary>
        public bool HasType => Schema != null;

        /// <summary>Gets a value indicating whether this is success response.</summary>
        public bool IsSuccess => HttpUtilities.IsSuccessStatusCode(StatusCode);

        /// <summary>Gets a value indicating whether the response is of type date.</summary>
        public bool IsDate =>
            (_response.ActualResponseSchema.Format == JsonFormatStrings.DateTime ||
            _response.ActualResponseSchema.Format == JsonFormatStrings.Date) &&
            _clientGeneratorBase.GetType(_response.ActualResponseSchema, IsNullable, "Response") != "string";

        /// <summary>Gets a value indicating whether this is a file response.</summary>
        public bool IsFile => Schema != null && Schema.ActualSchema.Type == JsonObjectType.File;

        /// <summary>Gets the response's exception description.</summary>
        public string ExceptionDescription => !string.IsNullOrEmpty(_response.Description) ?
            ConversionUtilities.ConvertToStringLiteral(_response.Description) :
            "A server side error occurred.";

        /// <summary>Gets the actual response schema.</summary>
        public JsonSchema4 ActualResponseSchema => _response.ActualResponseSchema;

        /// <summary>Gets the schema.</summary>
        private JsonSchema4 Schema => _response.Schema?.ActualSchema;

        /// <summary>Gets a value indicating whether the response is nullable.</summary>
        public bool IsNullable => _response.IsNullable(_clientGeneratorBase.BaseSettings.CodeGeneratorSettings.NullHandling);

        /// <summary>Gets a value indicating whether the response type inherits from exception.</summary>
        public bool InheritsExceptionSchema => _response.InheritsExceptionSchema(_exceptionSchema);

        // TODO: Find way to remove TypeScript only properties

        /// <summary>Gets or sets the data conversion code.</summary>
        public string DataConversionCode { get; set; }

        /// <summary>Gets or sets a value indicating whether to use a DTO class.</summary>
        public bool UseDtoClass { get; set; }
    }
}