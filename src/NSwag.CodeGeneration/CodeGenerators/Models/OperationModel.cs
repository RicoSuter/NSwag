//-----------------------------------------------------------------------
// <copyright file="OperationModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NJsonSchema.CodeGeneration;

namespace NSwag.CodeGeneration.CodeGenerators.Models
{
    internal class OperationModel
    {
        public string Id => Operation.OperationId;

        public string Path { get; set; }

        public SwaggerOperation Operation { get; set; }

        public SwaggerOperationMethod HttpMethod { get; set; }

        public string OperationName { get; set; }

        public string HttpMethodUpper => GeneratorBase.ConvertToUpperCamelCase(HttpMethod.ToString());

        public string HttpMethodLower => GeneratorBase.ConvertToLowerCamelCase(HttpMethod.ToString());

        public bool IsGetOrDelete => HttpMethod == SwaggerOperationMethod.Get || HttpMethod == SwaggerOperationMethod.Delete;

        public string OperationNameLower => GeneratorBase.ConvertToLowerCamelCase(OperationName);

        public string OperationNameUpper => GeneratorBase.ConvertToUpperCamelCase(OperationName);

        public string ResultType { get; set; }

        public bool HasResultType { get; set; }

        public string ResultDescription { get; set; }

        public bool HasResultDescription => !string.IsNullOrEmpty(ResultDescription);

        public string ExceptionType { get; set; }

        public List<ResponseModel> Responses { get; set; }

        public ResponseModel DefaultResponse { get; set; }

        public IEnumerable<ParameterModel> Parameters { get; set; }
        
        public bool HasDefaultResponse => DefaultResponse != null;

        public bool HasOnlyDefaultResponse => Responses.Count == 0 && HasDefaultResponse;
        
        public bool HasContent => ContentParameter != null;

        public ParameterModel ContentParameter => Parameters.SingleOrDefault(p => p.Kind == SwaggerParameterKind.Body);

        public IEnumerable<ParameterModel> PathParameters => Parameters.Where(p => p.Kind == SwaggerParameterKind.Path);

        public IEnumerable<ParameterModel> QueryParameters => Parameters.Where(p => p.Kind == SwaggerParameterKind.Query);

        public IEnumerable<ParameterModel> HeaderParameters => Parameters.Where(p => p.Kind == SwaggerParameterKind.Header);

        public string Summary => GeneratorBase.RemoveLineBreaks(Operation.Summary);

        public bool HasSummary => !string.IsNullOrEmpty(Summary);

        public bool HasDocumentation => HasSummary || HasResultDescription || Parameters.Any(p => p.HasDescription);
    }
}