//-----------------------------------------------------------------------
// <copyright file="ResponseModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NJsonSchema;

namespace NSwag.CodeGeneration.CodeGenerators.Models
{
    /// <summary>The response template model.</summary>
    public class ResponseModel
    {
        private readonly SwaggerResponse _response;
        private readonly ClientGeneratorBase _clientGeneratorBase;

        /// <summary>Initializes a new instance of the <see cref="ResponseModel" /> class.</summary>
        /// <param name="response">The response.</param>
        /// <param name="clientGeneratorBase">The client generator base.</param>
        public ResponseModel(KeyValuePair<string, SwaggerResponse> response, ClientGeneratorBase clientGeneratorBase)
        {
            _response = response.Value;
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
        public bool IsDate => _clientGeneratorBase.GetType(_response.ActualResponseSchema, IsNullable, "Response") == "Date";

        /// <summary>Gets a value indicating whether this is a file response.</summary>
        public bool IsFile => Schema != null && Schema.ActualSchema.Type == JsonObjectType.File;

        /// <summary>Gets the actual response schema.</summary>
        public JsonSchema4 ActualResponseSchema => _response.ActualResponseSchema;

        /// <summary>Gets the schema.</summary>
        private JsonSchema4 Schema => _response.Schema?.ActualSchema;

        /// <summary>Gets a value indicating whether the response is nullable.</summary>
        public bool IsNullable => _response.IsNullable(_clientGeneratorBase.BaseSettings.CodeGeneratorSettings.NullHandling);

        /// <summary>Gets a value indicating whether the response type inherits from exception.</summary>
        public bool HasExceptionSchema => _response.HasExceptionSchema;

        // TODO: Find way to remove TypeScript only properties

        /// <summary>Gets or sets the data conversion code.</summary>
        public string DataConversionCode { get; set; }

        /// <summary>Gets or sets a value indicating whether to use a DTO class.</summary>
        public bool UseDtoClass { get; set; }
    }
}