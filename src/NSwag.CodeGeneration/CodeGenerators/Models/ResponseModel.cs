//-----------------------------------------------------------------------
// <copyright file="ResponseModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;

namespace NSwag.CodeGeneration.CodeGenerators.Models
{
    internal class ResponseModel
    {
        private readonly ClientGeneratorBase _clientGeneratorBase;

        public ResponseModel(ClientGeneratorBase clientGeneratorBase)
        {
            _clientGeneratorBase = clientGeneratorBase;
        }

        public JsonSchema4 Schema { get; set; }

        public string StatusCode { get; set; }
        
        public string Type => _clientGeneratorBase.GetType(Schema, "Response");

        public bool HasType => Schema != null; 

        public bool IsSuccess => HttpUtilities.IsSuccessStatusCode(StatusCode);

        public bool IsDate => _clientGeneratorBase.GetType(Schema, "Response") == "Date";

        public bool IsFile => Schema != null && Schema.ActualSchema.Type == JsonObjectType.File;
    }
}