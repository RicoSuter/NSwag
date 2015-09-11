//-----------------------------------------------------------------------
// <copyright file="SwaggerToTypeScriptGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.TypeScript;

namespace NSwag.CodeGeneration.ClientGenerators.TypeScript
{
    /// <summary>Generates the CSharp service client code. </summary>
    public class SwaggerToTypeScriptGenerator : GeneratorBase
    {
        private readonly SwaggerService _service;
        private readonly TypeScriptTypeResolver _resolver;

        /// <summary>Initializes a new instance of the <see cref="SwaggerToTypeScriptGenerator"/> class.</summary>
        /// <param name="service">The service.</param>
        /// <exception cref="ArgumentNullException"><paramref name="service"/> is <see langword="null" />.</exception>
        public SwaggerToTypeScriptGenerator(SwaggerService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            Class = "Client";
            AsyncType = TypeScriptAsyncType.Callbacks;

            _service = service;
            _resolver = new TypeScriptTypeResolver(_service.Definitions.Select(p => p.Value).ToArray());
        }

        /// <summary>Gets or sets the class name of the service client.</summary>
        public string Class { get; set; }

        /// <summary>Gets or sets the type of the asynchronism handling.</summary>
        public TypeScriptAsyncType AsyncType { get; set; }

        /// <summary>Gets the language.</summary>
        protected override string Language
        {
            get { return "TypeScript"; }
        }

        /// <summary>Generates the file.</summary>
        /// <returns>The file contents.</returns>
        public string GenerateFile()
        {
            _service.GenerateOperationIds();

            var operations = _service.Paths
                .SelectMany(pair => pair.Value.Select(p => new { Path = pair.Key, Method = p.Key, Operation = p.Value }))
                .Select(tuple =>
                {
                    var httpMethod = tuple.Method;
                    var operation = tuple.Operation;
                    var responses = operation.Responses.Select(r => new
                    {
                        StatusCode = r.Key,
                        IsSuccess = r.Key == "200",
                        Type = GetType(r.Value.Schema, "Response"),
                        TypeIsDate = GetType(r.Value.Schema, "Response") == "Date"
                    }).ToList();

                    var defaultResponse = responses.SingleOrDefault(r => r.StatusCode == "default");
                    if (defaultResponse != null)
                        responses.Remove(defaultResponse); 

                    return new
                    {
                        Name = operation.OperationId,
                        Method = httpMethod.ToString(),
                        OperationName = ConvertToLowerStart(operation.OperationId),
                        OperationNameUpper = ConvertToUpperStart(operation.OperationId),

                        ResultType = GetResultType(operation),
                        ExceptionType = GetExceptionType(operation),

                        Responses = responses,
                        DefaultResponse = defaultResponse, 
                        HasDefaultResponse = defaultResponse, 

                        Parameters = operation.Parameters.Select(parameter => new
                        {
                            Name = parameter.Name,
                            Type = _resolver.Resolve(parameter.ActualSchema, parameter.IsRequired, parameter.Name),
                            IsLast = operation.Parameters.LastOrDefault() == parameter
                        }).ToList(),

                        HasContent = operation.Parameters.Any(p => p.Kind == SwaggerParameterKind.body),
                        ContentParameter = operation.Parameters.SingleOrDefault(p => p.Kind == SwaggerParameterKind.body),

                        PlaceholderParameters = operation.Parameters.Where(p => p.Kind == SwaggerParameterKind.path).Select(p => new
                        {
                            p.Name,
                            IsDate = p.Format == JsonFormatStrings.DateTime
                        }),

                        QueryParameters = operation.Parameters.Where(p => p.Kind == SwaggerParameterKind.query).Select(p => new
                        {
                            p.Name,
                            IsDate = p.Format == JsonFormatStrings.DateTime
                        }),

                        Target = tuple.Path
                    };
                }).ToList();

            var template = LoadTemplate(AsyncType == TypeScriptAsyncType.Callbacks ? "Callbacks" : "Q");
            template.Add("class", Class);
            template.Add("operations", operations);
            template.Add("hasOperations", operations.Any());
            template.Add("baseUrl", _service.BaseUrl);
            template.Add("toolchain", SwaggerService.ToolchainVersion);
            template.Add("interfaces", _resolver.GenerateInterfaces());

            return template.Render().Replace("\r", string.Empty).Replace("\n\n\n\n", "\n\n").Replace("\n\n\n", "\n\n");
        }

        private string GetExceptionType(SwaggerOperation operation)
        {
            if (operation.Responses.Count(r => r.Key != "200") != 1)
                return "any";

            return GetType(operation.Responses.Single(r => r.Key != "200").Value.Schema, "Exception");
        }

        private string GetResultType(SwaggerOperation operation)
        {
            if (operation.Responses.Count(r => r.Key == "200") == 0)
                return "void";

            if (operation.Responses.Count(r => r.Key == "200") != 1)
                return "any";

            var response = operation.Responses.Single(r => r.Key == "200").Value;
            return GetType(response.Schema, "Response");
        }

        private string GetType(JsonSchema4 type, string typeNameHint)
        {
            if (type == null)
                return "any";

            return _resolver.Resolve(type.ActualSchema, true, typeNameHint);
        }
    }
}
