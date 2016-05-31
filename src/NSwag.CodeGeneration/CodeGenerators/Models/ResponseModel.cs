//-----------------------------------------------------------------------
// <copyright file="ResponseModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using NJsonSchema;

namespace NSwag.CodeGeneration.CodeGenerators.Models
{
    internal class ResponseModel
    {
        private readonly SwaggerResponse _response;
        private readonly ClientGeneratorBase _clientGeneratorBase;

        public ResponseModel(KeyValuePair<string, SwaggerResponse> response, ClientGeneratorBase clientGeneratorBase)
        {
            _response = response.Value;
            _clientGeneratorBase = clientGeneratorBase;

            StatusCode = response.Key;
        }

        public string StatusCode { get; }
        
        public string Type => _clientGeneratorBase.GetType(_response.ActualResponseSchema, _response.IsNullable, "Response");

        public bool HasType => Schema != null; 

        public bool IsSuccess => HttpUtilities.IsSuccessStatusCode(StatusCode);

        public bool IsDate => _clientGeneratorBase.GetType(_response.ActualResponseSchema, _response.IsNullable, "Response") == "Date";

        public bool IsFile => Schema != null && Schema.ActualSchema.Type == JsonObjectType.File;

        public JsonSchema4 ActualResponseSchema => _response.ActualResponseSchema;

        private JsonSchema4 Schema => _response.Schema?.ActualSchema;

        public bool IsNullable => Schema.ActualSchema?.IsNullable ?? false;

        // TODO: Find way to remove TypeScript only properties

        public string DataConversionCode { get; set; }

        public bool UseDtoClass { get; set; }
    }
}