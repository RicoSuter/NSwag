//-----------------------------------------------------------------------
// <copyright file="ResponseModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.CodeGenerators.Models
{
    internal class ResponseModel
    {
        public string Type { get; set; }

        public string StatusCode { get; set; }
        
        public bool IsSuccess { get; set; }

        public bool TypeIsDate { get; set; }

        public bool HasType { get; set; }
    }
}