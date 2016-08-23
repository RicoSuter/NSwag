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
    /// <summary>This is model which contains information about single operation. It will be passed into <see cref="ITemplate"/> to generate client.</summary>
    public class OperationModel
    {
        /// <summary>Gets the identifier.</summary>
        public string Id => Operation.OperationId;

        /// <summary>Gets or sets the path.</summary>
        public string Path { get; set; }

        /// <summary>Gets or sets the Swagger operation.</summary>
        public SwaggerOperation Operation { get; set; }

        /// <summary>Gets or sets the HTTP method.</summary>
        public SwaggerOperationMethod HttpMethod { get; set; }

        /// <summary>Gets or sets the name of the operation.</summary>
        public string OperationName { get; set; }

        /// <summary>Gets the HTTP method as upper case.</summary>
        public string HttpMethodUpper => ConversionUtilities.ConvertToUpperCamelCase(HttpMethod.ToString(), false);

        /// <summary>Gets the HTTP method as lower case.</summary>
        public string HttpMethodLower => ConversionUtilities.ConvertToLowerCamelCase(HttpMethod.ToString(), false);

        /// <summary>Gets a value indicating whether this operation is get or delete method.</summary>
        public bool IsGetOrDelete => HttpMethod == SwaggerOperationMethod.Get || HttpMethod == SwaggerOperationMethod.Delete;

        /// <summary>Gets the operation name as lower case.</summary>
        public string OperationNameLower => ConversionUtilities.ConvertToLowerCamelCase(OperationName, false);

        /// <summary>Gets the operation name as upper case.</summary>
        public string OperationNameUpper => ConversionUtilities.ConvertToUpperCamelCase(OperationName, false);

        /// <summary>Gets or sets the type of the result.</summary>
        public string ResultType { get; set; }

        /// <summary>Gets or sets a value indicating whether this operation has result type.</summary>
        public bool HasResultType { get; set; }

        /// <summary>Gets or sets the result description.</summary>
        public string ResultDescription { get; set; }

        /// <summary>Gets a value indicating whether this operation has result description.</summary>
        public bool HasResultDescription => !string.IsNullOrEmpty(ResultDescription);

        /// <summary>Gets or sets the type of the exception if any.</summary>
        public string ExceptionType { get; set; }

        /// <summary>Gets or sets the responses.</summary>
        public List<ResponseModel> Responses { get; set; }

        /// <summary>Gets or sets the default response.</summary>
        public ResponseModel DefaultResponse { get; set; }

        /// <summary>Gets or sets the parameters.</summary>
        public IEnumerable<ParameterModel> Parameters { get; set; }

        /// <summary>Gets a value indicating whether this operation has default response.</summary>
        public bool HasDefaultResponse => DefaultResponse != null;

        /// <summary>Gets a value indicating whether this operation has only default response.</summary>
        public bool HasOnlyDefaultResponse => Responses.Count == 0 && HasDefaultResponse;

        /// <summary>Gets a value indicating whether this operation has content.</summary>
        public bool HasContent => ContentParameter != null;

        /// <summary>Gets the content parameter.</summary>
        public ParameterModel ContentParameter => Parameters.SingleOrDefault(p => p.Kind == SwaggerParameterKind.Body);

        /// <summary>Gets the path parameters.</summary>
        public IEnumerable<ParameterModel> PathParameters => Parameters.Where(p => p.Kind == SwaggerParameterKind.Path);

        /// <summary>Gets the query parameters.</summary>
        public IEnumerable<ParameterModel> QueryParameters => Parameters.Where(p => p.Kind == SwaggerParameterKind.Query);

        /// <summary>Gets the header parameters.</summary>
        public IEnumerable<ParameterModel> HeaderParameters => Parameters.Where(p => p.Kind == SwaggerParameterKind.Header);

        /// <summary>Gets the form parameters.</summary>
        public IEnumerable<ParameterModel> FormParameters => Parameters.Where(p => p.Kind == SwaggerParameterKind.FormData);

        /// <summary>Gets the summary.</summary>
        public string Summary => ConversionUtilities.TrimWhiteSpaces(Operation.Summary);

        /// <summary>Gets a value indicating whether this operation has summary.</summary>
        public bool HasSummary => !string.IsNullOrEmpty(Summary);

        /// <summary>Gets a value indicating whether this operation has documentation.</summary>
        public bool HasDocumentation => HasSummary || HasResultDescription || Parameters.Any(p => p.HasDescription) || Operation.IsDeprecated;

        /// <summary>Gets or sets a value indicating whether this operation has form parameters.</summary>
        public bool HasFormParameters { get; set; }

        /// <summary>Gets a value indicating whether this operation is deprecated.</summary>
        public bool IsDeprecated => Operation.IsDeprecated;
    }
}