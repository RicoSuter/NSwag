//-----------------------------------------------------------------------
// <copyright file="SwaggerToTypeScriptGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.ClientGenerators.Models;

namespace NSwag.CodeGeneration.ClientGenerators.TypeScript
{
    /// <summary>Generates the CSharp service client code. </summary>
    public class SwaggerToTypeScriptGenerator : ClientGeneratorBase
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

            Class = "{controller}Client";
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
        public override string GenerateFile()
        {
            return GenerateFile(_service, _resolver);
        }

        internal override string RenderFile(string clientCode)
        {
            var template = LoadTemplate("File");
            template.Add("toolchain", SwaggerService.ToolchainVersion);
            template.Add("clients", clientCode);
            template.Add("interfaces", _resolver.GenerateTypes());
            return template.Render();
        }

        internal override string RenderClientCode(string controllerName, IEnumerable<OperationModel> operations)
        {
            var template = LoadTemplate(AsyncType == TypeScriptAsyncType.Callbacks ? "Callbacks" : "Q");
            template.Add("class", Class.Replace("{controller}", ConvertToUpperStartIdentifier(controllerName)));
            template.Add("operations", operations);
            template.Add("hasOperations", operations.Any());
            template.Add("baseUrl", _service.BaseUrl);
            return template.Render();
        }

        internal override string GetExceptionType(SwaggerOperation operation)
        {
            if (operation.Responses.Count(r => r.Key != "200") != 1)
                return "any";

            return GetType(operation.Responses.Single(r => r.Key != "200").Value.Schema, "Exception");
        }

        internal override string GetResultType(SwaggerOperation operation)
        {
            if (operation.Responses.Count(r => r.Key == "200") == 0)
                return "void";

            if (operation.Responses.Count(r => r.Key == "200") != 1)
                return "any";

            var response = GetOkResponse(operation);
            return GetType(response.Schema, "Response");
        }

        internal override string GetType(JsonSchema4 schema, string typeNameHint)
        {
            if (schema == null)
                return "any";

            if (schema.IsAnyType)
                return "any";

            return _resolver.Resolve(schema.ActualSchema, true, typeNameHint);
        }
    }
}
