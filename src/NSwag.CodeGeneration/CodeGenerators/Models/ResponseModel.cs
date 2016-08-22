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
    /// <summary>
    /// 
    /// </summary>
    public class ResponseModel
    {
        private readonly SwaggerResponse _response;
        private readonly ClientGeneratorBase _clientGeneratorBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseModel"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="clientGeneratorBase">The client generator base.</param>
        public ResponseModel(KeyValuePair<string, SwaggerResponse> response, ClientGeneratorBase clientGeneratorBase)
        {
            _response = response.Value;
            _clientGeneratorBase = clientGeneratorBase;

            StatusCode = response.Key;
        }

        /// <summary>
        /// Gets the status code.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        public string StatusCode { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type => _clientGeneratorBase.GetType(_response.ActualResponseSchema, IsNullable, "Response");

        /// <summary>
        /// Gets a value indicating whether this instance has type.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has type; otherwise, <c>false</c>.
        /// </value>
        public bool HasType => Schema != null;

        /// <summary>
        /// Gets a value indicating whether this instance is success.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is success; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuccess => HttpUtilities.IsSuccessStatusCode(StatusCode);

        /// <summary>
        /// Gets a value indicating whether this instance is date.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is date; otherwise, <c>false</c>.
        /// </value>
        public bool IsDate => _clientGeneratorBase.GetType(_response.ActualResponseSchema, IsNullable, "Response") == "Date";

        /// <summary>
        /// Gets a value indicating whether this instance is file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is file; otherwise, <c>false</c>.
        /// </value>
        public bool IsFile => Schema != null && Schema.ActualSchema.Type == JsonObjectType.File;

        /// <summary>
        /// Gets the actual response schema.
        /// </summary>
        /// <value>
        /// The actual response schema.
        /// </value>
        public JsonSchema4 ActualResponseSchema => _response.ActualResponseSchema;

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <value>
        /// The schema.
        /// </value>
        private JsonSchema4 Schema => _response.Schema?.ActualSchema;

        /// <summary>
        /// Gets a value indicating whether this instance is nullable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is nullable; otherwise, <c>false</c>.
        /// </value>
        public bool IsNullable => _response.IsNullable(_clientGeneratorBase.BaseSettings.CodeGeneratorSettings.NullHandling);

        /// <summary>
        /// Gets a value indicating whether [type inherits from exception].
        /// </summary>
        /// <value>
        /// <c>true</c> if [type inherits from exception]; otherwise, <c>false</c>.
        /// </value>
        public bool TypeInheritsFromException => _response
            .ActualResponseSchema?
            .InheritedSchemas
            .Any(s => new[] { "innerexception", "message", "source", "stacktrace" }.All(p => s.ActualSchema.Properties.Any(i => i.Key.ToLowerInvariant() == p))) == true;

        // TODO: Find way to remove TypeScript only properties

        /// <summary>
        /// Gets or sets the data conversion code.
        /// </summary>
        /// <value>
        /// The data conversion code.
        /// </value>
        public string DataConversionCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use dto class].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use dto class]; otherwise, <c>false</c>.
        /// </value>
        public bool UseDtoClass { get; set; }
    }
}