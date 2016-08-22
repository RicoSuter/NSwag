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
    /// <summary>
    /// 
    /// </summary>
    public class OperationModel
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id => Operation.OperationId;

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public SwaggerOperation Operation { get; set; }

        /// <summary>
        /// Gets or sets the HTTP method.
        /// </summary>
        /// <value>
        /// The HTTP method.
        /// </value>
        public SwaggerOperationMethod HttpMethod { get; set; }

        /// <summary>
        /// Gets or sets the name of the operation.
        /// </summary>
        /// <value>
        /// The name of the operation.
        /// </value>
        public string OperationName { get; set; }

        /// <summary>
        /// Gets the HTTP method upper.
        /// </summary>
        /// <value>
        /// The HTTP method upper.
        /// </value>
        public string HttpMethodUpper => ConversionUtilities.ConvertToUpperCamelCase(HttpMethod.ToString(), false);

        /// <summary>
        /// Gets the HTTP method lower.
        /// </summary>
        /// <value>
        /// The HTTP method lower.
        /// </value>
        public string HttpMethodLower => ConversionUtilities.ConvertToLowerCamelCase(HttpMethod.ToString(), false);

        /// <summary>
        /// Gets a value indicating whether this instance is get or delete.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is get or delete; otherwise, <c>false</c>.
        /// </value>
        public bool IsGetOrDelete => HttpMethod == SwaggerOperationMethod.Get || HttpMethod == SwaggerOperationMethod.Delete;

        /// <summary>
        /// Gets the operation name lower.
        /// </summary>
        /// <value>
        /// The operation name lower.
        /// </value>
        public string OperationNameLower => ConversionUtilities.ConvertToLowerCamelCase(OperationName, false);

        /// <summary>
        /// Gets the operation name upper.
        /// </summary>
        /// <value>
        /// The operation name upper.
        /// </value>
        public string OperationNameUpper => ConversionUtilities.ConvertToUpperCamelCase(OperationName, false);

        /// <summary>
        /// Gets or sets the type of the result.
        /// </summary>
        /// <value>
        /// The type of the result.
        /// </value>
        public string ResultType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has result type.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has result type; otherwise, <c>false</c>.
        /// </value>
        public bool HasResultType { get; set; }

        /// <summary>
        /// Gets or sets the result description.
        /// </summary>
        /// <value>
        /// The result description.
        /// </value>
        public string ResultDescription { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has result description.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has result description; otherwise, <c>false</c>.
        /// </value>
        public bool HasResultDescription => !string.IsNullOrEmpty(ResultDescription);

        /// <summary>
        /// Gets or sets the type of the exception.
        /// </summary>
        /// <value>
        /// The type of the exception.
        /// </value>
        public string ExceptionType { get; set; }

        /// <summary>
        /// Gets or sets the responses.
        /// </summary>
        /// <value>
        /// The responses.
        /// </value>
        public List<ResponseModel> Responses { get; set; }

        /// <summary>
        /// Gets or sets the default response.
        /// </summary>
        /// <value>
        /// The default response.
        /// </value>
        public ResponseModel DefaultResponse { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public IEnumerable<ParameterModel> Parameters { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has default response.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has default response; otherwise, <c>false</c>.
        /// </value>
        public bool HasDefaultResponse => DefaultResponse != null;

        /// <summary>
        /// Gets a value indicating whether this instance has only default response.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has only default response; otherwise, <c>false</c>.
        /// </value>
        public bool HasOnlyDefaultResponse => Responses.Count == 0 && HasDefaultResponse;

        /// <summary>
        /// Gets a value indicating whether this instance has content.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has content; otherwise, <c>false</c>.
        /// </value>
        public bool HasContent => ContentParameter != null;

        /// <summary>
        /// Gets the content parameter.
        /// </summary>
        /// <value>
        /// The content parameter.
        /// </value>
        public ParameterModel ContentParameter => Parameters.SingleOrDefault(p => p.Kind == SwaggerParameterKind.Body);

        /// <summary>
        /// Gets the path parameters.
        /// </summary>
        /// <value>
        /// The path parameters.
        /// </value>
        public IEnumerable<ParameterModel> PathParameters => Parameters.Where(p => p.Kind == SwaggerParameterKind.Path);

        /// <summary>
        /// Gets the query parameters.
        /// </summary>
        /// <value>
        /// The query parameters.
        /// </value>
        public IEnumerable<ParameterModel> QueryParameters => Parameters.Where(p => p.Kind == SwaggerParameterKind.Query);

        /// <summary>
        /// Gets the header parameters.
        /// </summary>
        /// <value>
        /// The header parameters.
        /// </value>
        public IEnumerable<ParameterModel> HeaderParameters => Parameters.Where(p => p.Kind == SwaggerParameterKind.Header);

        /// <summary>
        /// Gets the form parameters.
        /// </summary>
        /// <value>
        /// The form parameters.
        /// </value>
        public IEnumerable<ParameterModel> FormParameters => Parameters.Where(p => p.Kind == SwaggerParameterKind.FormData);

        /// <summary>
        /// Gets the summary.
        /// </summary>
        /// <value>
        /// The summary.
        /// </value>
        public string Summary => ConversionUtilities.TrimWhiteSpaces(Operation.Summary);

        /// <summary>
        /// Gets a value indicating whether this instance has summary.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has summary; otherwise, <c>false</c>.
        /// </value>
        public bool HasSummary => !string.IsNullOrEmpty(Summary);

        /// <summary>
        /// Gets a value indicating whether this instance has documentation.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has documentation; otherwise, <c>false</c>.
        /// </value>
        public bool HasDocumentation => HasSummary || HasResultDescription || Parameters.Any(p => p.HasDescription) || Operation.IsDeprecated;

        /// <summary>
        /// Gets or sets a value indicating whether this instance has form parameters.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has form parameters; otherwise, <c>false</c>.
        /// </value>
        public bool HasFormParameters { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is deprecated.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is deprecated; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeprecated => Operation.IsDeprecated;
    }
}