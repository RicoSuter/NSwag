//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;

namespace NSwag.CodeGeneration.ClientGenerators.CSharp
{
    /// <summary>Generates the CSharp service client code. </summary>
    public class SwaggerToCSharpGenerator : GeneratorBase
    {
        private readonly SwaggerService _service;
        //private readonly Dictionary<string, CSharpClassGenerator> _types;
        private readonly CSharpTypeResolver _resolver;

        /// <summary>Initializes a new instance of the <see cref="SwaggerToCSharpGenerator"/> class.</summary>
        /// <param name="service">The service.</param>
        /// <exception cref="ArgumentNullException"><paramref name="service"/> is <see langword="null" />.</exception>
        public SwaggerToCSharpGenerator(SwaggerService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            _service = service;
            _resolver = new CSharpTypeResolver(_service.Definitions.Select(p => p.Value).ToArray());
//            _types = _service.Definitions.ToDictionary(t => t.Key, t => new CSharpClassGenerator(t.Value, _resolver));
        }

        /// <summary>Gets or sets the class name of the service client.</summary>
        public string Class { get; set; }

        /// <summary>Gets or sets the namespace.</summary>
        public string Namespace { get; set; }

        /// <summary>Gets the language.</summary>
        protected override string Language
        {
            get { return "CSharp"; }
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
                    var httpMethod = ConvertToUpperStart(tuple.Method.ToString());
                    var operation = tuple.Operation;
                    var responses = operation.Responses.Select(r => new
                    {
                        StatusCode = r.Key,
                        IsSuccess = r.Key == "200",
                        Type = GetType(r.Value.Schema, "Response"),
                    }).ToList();

                    var defaultResponse = responses.SingleOrDefault(r => r.StatusCode == "default");
                    if (defaultResponse != null)
                        responses.Remove(defaultResponse);

                    return new
                    {
                        Name = operation.OperationId,

                        Method = httpMethod.ToString(),
                        IsGetOrDelete = tuple.Method == SwaggerOperationMethod.get || tuple.Method == SwaggerOperationMethod.delete, 
                        MethodName = ConvertToUpperStart(operation.OperationId),

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

                        PlaceholderParameters = operation.Parameters.Where(p => p.Kind == SwaggerParameterKind.path),
                        QueryParameters = operation.Parameters.Where(p => p.Kind == SwaggerParameterKind.query),

                        Target = tuple.Path
                    };
                }).ToList();

            var template = LoadTemplate("File");
            template.Add("class", Class);
            template.Add("namespace", Namespace);
            template.Add("baseUrl", _service.BaseUrl);
            template.Add("operations", operations);
            template.Add("hasOperations", operations.Any());
            template.Add("toolchain", SwaggerService.ToolchainVersion);
            template.Add("classes", _resolver.GenerateClasses());

            return template.Render()
                .Replace("\r", string.Empty)
                .Replace("\n\n\n\n", "\n\n")
                .Replace("\n\n\n", "\n\n");
        }

        private string GetExceptionType(SwaggerOperation operation)
        {
            if (operation.Responses.Count(r => r.Key != "200") != 1)
                return "Exception";

            return GetType(operation.Responses.Single(r => r.Key != "200").Value.Schema, "Exception");
        }

        private string GetResultType(SwaggerOperation operation)
        {
            if (operation.Responses.Count(r => r.Key == "200") == 0)
                return "void";

            if (operation.Responses.Count(r => r.Key == "200") != 1)
                return "object";

            var response = operation.Responses.Single(r => r.Key == "200").Value;
            return GetType(response.Schema, "Response");
        }

        private string GetType(JsonSchema4 schema, string typeNameHint)
        {
            if (schema == null)
                return "string";

            return _resolver.Resolve(schema.ActualSchema, true, typeNameHint);
        }
    }
}
