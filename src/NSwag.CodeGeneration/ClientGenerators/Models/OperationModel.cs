//-----------------------------------------------------------------------
// <copyright file="OperationModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;

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

        public string ExceptionType { get; set; }

        public List<ResponseModel> Responses { get; set; }

        public ResponseModel DefaultResponse { get; set; }

        public bool HasDefaultResponse { get { return DefaultResponse != null; } }

        public List<ParameterModel> Parameters { get; set; }

        public bool HasContent { get { return ContentParameter != null; } }

        public ParameterModel ContentParameter { get; set; }

        public IEnumerable<ParameterModel> PlaceholderParameters { get; set; }

        public IEnumerable<ParameterModel> QueryParameters { get; set; }

        public string Path { get; set; }

        public bool IsGetOrDelete { get; set; }
        public string HttpMethodLower { get; set; }
    }
}