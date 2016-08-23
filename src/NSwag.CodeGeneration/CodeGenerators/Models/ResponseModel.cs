//-----------------------------------------------------------------------
// <copyright file="ResponseModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using NJsonSchema;

namespace NSwag.CodeGeneration.CodeGenerators.Models
{
    /// <summary>This is response model which will be passed into client generation template.</summary>
    public class ResponseModel
    {
        private readonly SwaggerResponse _response;
        private readonly ClientGeneratorBase _clientGeneratorBase;

        /// <summary>Initializes a new instance of the <see cref="ResponseModel"/> class.</summary>
        /// <param name="response">The response.</param>
        /// <param name="clientGeneratorBase">The client generator base.</param>
        public ResponseModel(KeyValuePair<string, SwaggerResponse> response, ClientGeneratorBase clientGeneratorBase)
        {
            _response = response.Value;
            _clientGeneratorBase = clientGeneratorBase;

            StatusCode = response.Key;
        }

        /// <summary>Gets the status code.</summary>
        public string StatusCode { get; }

        /// <summary>Gets the type.</summary>
        public string Type => _clientGeneratorBase.GetType(_response.ActualResponseSchema, IsNullable, "Response");

        /// <summary>Gets a value indicating whether response has type.</summary>
        public bool HasType => Schema != null;

        /// <summary>Gets a value indicating whether response was successful.</summary>
        public bool IsSuccess => HttpUtilities.IsSuccessStatusCode(StatusCode);

        /// <summary>Gets a value indicating whether response has date.</summary>
        public bool IsDate => _clientGeneratorBase.GetType(_response.ActualResponseSchema, IsNullable, "Response") == "Date";

        /// <summary>Gets a value indicating whether response has file.</summary>
        public bool IsFile => Schema != null && Schema.ActualSchema.Type == JsonObjectType.File;

        /// <summary>Gets the actual response schema.</summary>
        public JsonSchema4 ActualResponseSchema => _response.ActualResponseSchema;

        /// <summary>Gets the schema.</summary>
        private JsonSchema4 Schema => _response.Schema?.ActualSchema;

        /// <summary> Gets a value indicating whether response type is nullable.</summary>
        public bool IsNullable => _response.IsNullable(_clientGeneratorBase.BaseSettings.CodeGeneratorSettings.NullHandling);

        /// <summary>Gets a value indicating whether response type inherits from <see cref="Exception"/>.</summary>
        public bool TypeInheritsFromException => _response
            .ActualResponseSchema?
            .InheritedSchemas
            .Any(s => new[] { "innerexception", "message", "source", "stacktrace" }.All(p => s.ActualSchema.Properties.Any(i => i.Key.ToLowerInvariant() == p))) == true;

        // TODO: Find way to remove TypeScript only properties

        /// <summary>Gets or sets the data conversion code.</summary>
        public string DataConversionCode { get; set; }

        /// <summary>Gets or sets a value indicating whether DTO class should be used.</summary>
        public bool UseDtoClass { get; set; }
    }
}