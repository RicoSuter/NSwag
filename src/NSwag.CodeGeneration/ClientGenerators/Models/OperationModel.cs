//-----------------------------------------------------------------------
// <copyright file="OperationModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace NSwag.CodeGeneration.ClientGenerators.Models
{
    internal class OperationModel
    {
        public string Id { get; set; }

        public string HttpMethodUpper { get; set; }

        public string MvcActionName { get; set; }

        public string MvcControllerName { get; set; }

        public string OperationNameLower { get; set; }

        public string OperationNameUpper { get; set; }

        public string ResultType { get; set; }

        public string ResultDescription { get; set; }

        public bool HasResultDescription
        {
            get { return !string.IsNullOrEmpty(ResultDescription); }
        }

        public string ExceptionType { get; set; }

        public List<ResponseModel> Responses { get; set; }

        public ResponseModel DefaultResponse { get; set; }

        public bool HasDefaultResponse { get { return DefaultResponse != null; } }

        public IEnumerable<ParameterModel> Parameters { get; set; }

        public bool HasContent { get { return ContentParameter != null; } }

        public ParameterModel ContentParameter
        {
            get
            {
                return Parameters.SingleOrDefault(p => p.Kind == SwaggerParameterKind.Body);
            }
        }

        public IEnumerable<ParameterModel> PathParameters
        {
            get { return Parameters.Where(p => p.Kind == SwaggerParameterKind.Path); }
        }

        public IEnumerable<ParameterModel> QueryParameters
        {
            get { return Parameters.Where(p => p.Kind == SwaggerParameterKind.Query); }
        }

        public IEnumerable<ParameterModel> HeaderParameters
        {
            get { return Parameters.Where(p => p.Kind == SwaggerParameterKind.Header); }
        }

        public string Path { get; set; }

        public bool IsGetOrDelete { get; set; }

        public string HttpMethodLower { get; set; }

        public string Summary { get; set; }

        public bool HasSummary
        {
            get { return !string.IsNullOrEmpty(Summary); }
        }

        public bool HasDocumentation
        {
            get { return HasSummary || HasResultDescription || Parameters.Any(p => p.HasDescription); }
        }
    }
}