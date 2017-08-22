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
using NJsonSchema.CodeGeneration;

namespace NSwag.CodeGeneration.Models
{
    /// <summary>The Swagger operation template model.</summary>
    public abstract class OperationModelBase<TParameterModel, TResponseModel> : IOperationModel
        where TParameterModel : ParameterModelBase
        where TResponseModel : ResponseModelBase
    {
        private readonly SwaggerOperation _operation;
        private readonly ITypeResolver _resolver;
        private readonly IClientGenerator _generator;
        private readonly ClientGeneratorBaseSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="OperationModelBase{TParameterModel, TResponseModel}"/> class.</summary>
        /// <param name="exceptionSchema">The exception schema.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="settings">The settings.</param>
        protected OperationModelBase(JsonSchema4 exceptionSchema, SwaggerOperation operation, ITypeResolver resolver, IClientGenerator generator, ClientGeneratorBaseSettings settings)
        {
            _operation = operation;
            _resolver = resolver;
            _generator = generator;
            _settings = settings;

            var responses = _operation.Responses
                .Select(response => CreateResponseModel(response.Key, response.Value, exceptionSchema, generator, settings))
                .ToList();

            var defaultResponse = responses.SingleOrDefault(r => r.StatusCode == "default");
            if (defaultResponse != null)
                responses.Remove(defaultResponse);

            Responses = responses;
            DefaultResponse = defaultResponse;
        }

        /// <summary>Creates the response model.</summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="response">The response.</param>
        /// <param name="exceptionSchema">The exception schema.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>The response model.</returns>
        protected abstract TResponseModel CreateResponseModel(string statusCode, SwaggerResponse response, JsonSchema4 exceptionSchema, IClientGenerator generator, ClientGeneratorBaseSettings settings);

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

        /// <summary>Gets the actual name of the operation (language specific).</summary>
        public abstract string ActualOperationName { get; }

        /// <summary>Gets the HTTP method in uppercase.</summary>
        public string HttpMethodUpper => ConversionUtilities.ConvertToUpperCamelCase(HttpMethod.ToString(), false);

        /// <summary>Gets the HTTP method in lowercase.</summary>
        public string HttpMethodLower => ConversionUtilities.ConvertToLowerCamelCase(HttpMethod.ToString(), false);

        /// <summary>Gets a value indicating whether the HTTP method is GET or DELETE or HEAD.</summary>
        public bool IsGetOrDeleteOrHead =>
            HttpMethod == SwaggerOperationMethod.Get ||
            HttpMethod == SwaggerOperationMethod.Delete ||
            HttpMethod == SwaggerOperationMethod.Head;

        /// <summary>Gets a value indicating whether the HTTP method is GET or HEAD.</summary>
        public bool IsGetOrHead => HttpMethod == SwaggerOperationMethod.Get || HttpMethod == SwaggerOperationMethod.Head;

        // TODO: Remove this (may not work correctly)
        /// <summary>Gets or sets a value indicating whether the operation has a result type (i.e. not void).</summary>
        public bool HasResultType
        {
            get
            {
                var response = GetSuccessResponse();
                return response?.ActualResponseSchema != null;
            }
        }

        /// <summary>Gets or sets the type of the result.</summary>
        public abstract string ResultType { get; }

        /// <summary>Gets the type of the unwrapped result type (without Task).</summary>
        public string UnwrappedResultType
        {
            get
            {
                var response = GetSuccessResponse();
                if (response?.ActualResponseSchema == null)
                    return "void";

                var isNullable = response.IsNullable(_settings.CodeGeneratorSettings.NullHandling);
                return _generator.GetTypeName(response.ActualResponseSchema, isNullable, "Response");
            }
        }

        /// <summary>Gets a value indicating whether the result has description.</summary>
        public bool HasResultDescription => !string.IsNullOrEmpty(ResultDescription);

        /// <summary>Gets or sets the result description.</summary>
        public string ResultDescription
        {
            get
            {
                var response = GetSuccessResponse();
                if (response != null)
                    return ConversionUtilities.TrimWhiteSpaces(response.Description);
                return null;
            }
        }

        /// <summary>Gets the name of the controller.</summary>
        public string ControllerName { get; set; }

        /// <summary>Gets or sets the type of the exception.</summary>
        public abstract string ExceptionType { get; }

        /// <summary>Gets or sets the responses.</summary>
        public List<TResponseModel> Responses { get; }

        /// <summary>Gets a value indicating whether the operation has default response.</summary>
        public bool HasDefaultResponse => DefaultResponse != null;

        /// <summary>Gets or sets the default response.</summary>
        public TResponseModel DefaultResponse { get; }

        /// <summary>Gets a value indicating whether the operation has an explicit success response defined.</summary>
        public bool HasSuccessResponse => Responses.Any(r => r.IsSuccess(this));

        /// <summary>Gets the success response.</summary>
        public TResponseModel SuccessResponse => Responses.FirstOrDefault(r => r.IsSuccess(this));

        /// <summary>Gets the responses.</summary>
        IEnumerable<ResponseModelBase> IOperationModel.Responses => Responses;

        /// <summary>Gets or sets the parameters.</summary>
        public IList<TParameterModel> Parameters { get; protected set; }

        /// <summary>Gets a value indicating whether the operation has only a default response.</summary>
        public bool HasOnlyDefaultResponse => Responses.Count == 0 && HasDefaultResponse;

        /// <summary>Gets a value indicating whether the operation has content parameter.</summary>
        public bool HasContent => ContentParameter != null;

        /// <summary>Gets a value indicating whether the the request has a body.</summary>
        public bool HasBody => HasContent || HasFormParameters;

        /// <summary>Gets the content parameter.</summary>
        public TParameterModel ContentParameter => Parameters.SingleOrDefault(p => p.Kind == SwaggerParameterKind.Body);

        /// <summary>Gets the path parameters.</summary>
        public IEnumerable<TParameterModel> PathParameters => Parameters.Where(p => p.Kind == SwaggerParameterKind.Path);

        /// <summary>Gets the query parameters.</summary>
        public IEnumerable<TParameterModel> QueryParameters => Parameters.Where(p => p.Kind == SwaggerParameterKind.Query || p.Kind == SwaggerParameterKind.ModelBinding);

        /// <summary>Gets a value indicating whether the operation has query parameters.</summary>
        public bool HasQueryParameters => QueryParameters.Any();

        /// <summary>Gets the header parameters.</summary>
        public IEnumerable<TParameterModel> HeaderParameters => Parameters.Where(p => p.Kind == SwaggerParameterKind.Header);

        /// <summary>Gets or sets a value indicating whether the accept header is defined in a parameter.</summary>
        public bool HasAcceptHeaderParameterParameter => HeaderParameters.Any(p => p.Name.ToLowerInvariant() == "accept");

        /// <summary>Gets or sets a value indicating whether the operation has form parameters.</summary>
        public bool HasFormParameters => _operation.ActualParameters.Any(p => p.Kind == SwaggerParameterKind.FormData);

        /// <summary>Gets the form parameters.</summary>
        public IEnumerable<TParameterModel> FormParameters => Parameters.Where(p => p.Kind == SwaggerParameterKind.FormData);

        /// <summary>Gets a value indicating whether the operation has summary.</summary>
        public bool HasSummary => !string.IsNullOrEmpty(Summary);

        /// <summary>Gets the summary text.</summary>
        public string Summary => ConversionUtilities.TrimWhiteSpaces(Operation.Summary);

        /// <summary>Gets a value indicating whether the operation has any documentation.</summary>
        public bool HasDocumentation => HasSummary || HasResultDescription || Parameters.Any(p => p.HasDescription) || Operation.IsDeprecated;

        /// <summary>Gets a value indicating whether the operation is deprecated.</summary>
        public bool IsDeprecated => Operation.IsDeprecated;

        /// <summary>Gets or sets a value indicating whether this operation has an XML body parameter.</summary>
        public bool HasXmlBodyParameter => Operation.ActualParameters.Any(p => p.IsXmlBodyParameter);

        /// <summary>Gets or sets a value indicating whether this operation has an binary body parameter.</summary>
        public bool HasBinaryBodyParameter => Operation.ActualParameters.Any(p => p.IsBinaryBodyParameter);

        /// <summary>Gets the mime type of the request body.</summary>
        public string Consumes
        {
            get
            {
                if (Operation.ActualConsumes?.Contains("application/json") == true)
                    return "application/json";

                return Operation.ActualConsumes?.FirstOrDefault() ?? "application/json";
            }
        }

        /// <summary>Gets the mime type of the response body.</summary>
        public string Produces
        {
            get
            {
                if (Operation.ActualProduces?.Contains("application/json") == true)
                    return "application/json";

                return Operation.ActualProduces?.FirstOrDefault() ?? "application/json";
            }
        }

        /// <summary>Gets a value indicating whether a file response is expected from one of the responses.</summary>
        public bool IsFile => _operation.AllResponses.Any(r => r.Value.Schema?.ActualSchema.Type == JsonObjectType.File);

        /// <summary>Gets the success response.</summary>
        /// <returns>The response.</returns>
        protected SwaggerResponse GetSuccessResponse()
        {
            if (_operation.Responses.Any(r => r.Key == "200"))
                return _operation.Responses.Single(r => r.Key == "200").Value;

            var response = _operation.Responses.FirstOrDefault(r => HttpUtilities.IsSuccessStatusCode(r.Key)).Value;
            if (response != null)
                return response;

            return _operation.Responses.FirstOrDefault(r => r.Key == "default").Value;
        }

        /// <summary>Gets the name of the parameter variable.</summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="allParameters">All parameters.</param>
        /// <returns>The parameter variable name.</returns>
        protected virtual string GetParameterVariableName(SwaggerParameter parameter, IEnumerable<SwaggerParameter> allParameters)
        {
            return _settings.ParameterNameGenerator.Generate(parameter, allParameters);
        }

        /// <summary>Resolves the type of the parameter.</summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The parameter type name.</returns>
        protected virtual string ResolveParameterType(SwaggerParameter parameter)
        {
            var schema = parameter.ActualSchema;

            if (parameter.IsXmlBodyParameter)
                return "string";

            if (parameter.CollectionFormat == SwaggerParameterCollectionFormat.Multi && !schema.Type.HasFlag(JsonObjectType.Array))
                schema = new JsonSchema4 { Type = JsonObjectType.Array, Item = schema };

            var typeNameHint = ConversionUtilities.ConvertToUpperCamelCase(parameter.Name, true);
            return _resolver.Resolve(schema, parameter.IsRequired == false || parameter.IsNullable(_settings.CodeGeneratorSettings.NullHandling), typeNameHint);
        }
    }
};