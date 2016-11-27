//-----------------------------------------------------------------------
// <copyright file="OperationModel.cs" company="NSwag">
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
    /// <summary>The Swagger operation template model.</summary>
    public class OperationModel
    {
        /// <summary>Gets or sets the operation.</summary>
        public SwaggerOperation Operation { get; set; }

        /// <summary>Gets the operation ID.</summary>
        public string Id => Operation.OperationId;

        /// <summary>Gets or sets the HTTP path (i.e. the absolute route).</summary>
        public string Path { get; set; }

        /// <summary>Gets or sets the HTTP method.</summary>
        public SwaggerOperationMethod HttpMethod { get; set; }

        /// <summary>Gets or sets the name of the operation.</summary>
        public string OperationName { get; set; }

        /// <summary>Gets the HTTP method in uppercase.</summary>
        public string HttpMethodUpper => ConversionUtilities.ConvertToUpperCamelCase(HttpMethod.ToString(), false);

        /// <summary>Gets the HTTP method in lowercase.</summary>
        public string HttpMethodLower => ConversionUtilities.ConvertToLowerCamelCase(HttpMethod.ToString(), false);

        /// <summary>Gets a value indicating whether the HTTP method is GET or DELETE.</summary>
        public bool IsGetOrDelete => HttpMethod == SwaggerOperationMethod.Get || HttpMethod == SwaggerOperationMethod.Delete;

        /// <summary>Gets a value indicating whether the HTTP method is GET or HEAD.</summary>
        public bool IsGetOrHead => HttpMethod == SwaggerOperationMethod.Get || HttpMethod == SwaggerOperationMethod.Head;

        /// <summary>Gets the operation name in lowercase.</summary>
        public string OperationNameLower => ConversionUtilities.ConvertToLowerCamelCase(OperationName, false);

        /// <summary>Gets the operation name in uppercase.</summary>
        public string OperationNameUpper => ConversionUtilities.ConvertToUpperCamelCase(OperationName, false);

        // TODO: Remove this (not may not work correctly)
        /// <summary>Gets or sets a value indicating whether the operation has a result type (i.e. not void).</summary>
        public bool HasResultType { get; set; }

        /// <summary>Gets or sets the type of the result.</summary>
        public string ResultType { get; set; }

        /// <summary>Gets a value indicating whether the result has description.</summary>
        public bool HasResultDescription => !string.IsNullOrEmpty(ResultDescription);

        /// <summary>Gets or sets the result description.</summary>
        public string ResultDescription { get; set; }

        /// <summary>Gets or sets the type of the exception.</summary>
        public string ExceptionType { get; set; }

        /// <summary>Gets or sets the responses.</summary>
        public List<ResponseModel> Responses { get; set; }

        /// <summary>Gets a value indicating whether the operation has default response.</summary>
        public bool HasDefaultResponse => DefaultResponse != null;

        /// <summary>Gets or sets the default response.</summary>
        public ResponseModel DefaultResponse { get; set; }

        /// <summary>Gets a value indicating whether the operation has an explicit success response defined.</summary>
        public bool HasSuccessResponse => Responses.Any(r => r.IsSuccess);

        /// <summary>Gets or sets the parameters.</summary>
        public IEnumerable<ParameterModel> Parameters { get; set; }

        /// <summary>Gets a value indicating whether the operation has only a default response.</summary>
        public bool HasOnlyDefaultResponse => Responses.Count == 0 && HasDefaultResponse;

        /// <summary>Gets a value indicating whether the operation has content parameter.</summary>
        public bool HasContent => ContentParameter != null;

        /// <summary>Gets the content parameter.</summary>
        public ParameterModel ContentParameter => Parameters.SingleOrDefault(p => p.Kind == SwaggerParameterKind.Body);

        /// <summary>Gets the path parameters.</summary>
        public IEnumerable<ParameterModel> PathParameters => Parameters.Where(p => p.Kind == SwaggerParameterKind.Path);

        /// <summary>Gets the query parameters.</summary>
        public IEnumerable<ParameterModel> QueryParameters => Parameters.Where(p => p.Kind == SwaggerParameterKind.Query || p.Kind == SwaggerParameterKind.ModelBinding);

        /// <summary>Gets the header parameters.</summary>
        public IEnumerable<ParameterModel> HeaderParameters => Parameters.Where(p => p.Kind == SwaggerParameterKind.Header);

        /// <summary>Gets or sets a value indicating whether the operation has form parameters.</summary>
        public bool HasFormParameters { get; set; }

        /// <summary>Gets the form parameters.</summary>
        public IEnumerable<ParameterModel> FormParameters => Parameters.Where(p => p.Kind == SwaggerParameterKind.FormData);

        /// <summary>Gets a value indicating whether the operation has summary.</summary>
        public bool HasSummary => !string.IsNullOrEmpty(Summary);

        /// <summary>Gets the summary text.</summary>
        public string Summary => ConversionUtilities.TrimWhiteSpaces(Operation.Summary);

        /// <summary>Gets a value indicating whether the operation has any documentation.</summary>
        public bool HasDocumentation => HasSummary || HasResultDescription || Parameters.Any(p => p.HasDescription) || Operation.IsDeprecated;

        /// <summary>Gets a value indicating whether the operation is deprecated.</summary>
        public bool IsDeprecated => Operation.IsDeprecated;
    }
}