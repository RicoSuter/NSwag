//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NSwag.CodeGeneration.ClientGenerators.Models;
using NSwag.CodeGeneration.ClientGenerators.TypeScript;

namespace NSwag.CodeGeneration.ClientGenerators.CSharp
{
    /// <summary>Generates the CSharp service client code. </summary>
    public class SwaggerToCSharpGenerator : ClientGeneratorBase
    {
        private readonly SwaggerService _service;
        private readonly SwaggerToCSharpTypeResolver _resolver;

        /// <summary>Initializes a new instance of the <see cref="SwaggerToCSharpGenerator" /> class.</summary>
        /// <param name="service">The service.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.ArgumentNullException">service</exception>
        /// <exception cref="ArgumentNullException"><paramref name="service" /> is <see langword="null" />.</exception>
        public SwaggerToCSharpGenerator(SwaggerService service, SwaggerToCSharpGeneratorSettings settings)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            Settings = settings; 

            _service = service;
            foreach (var definition in _service.Definitions)
                definition.Value.TypeName = definition.Key;

            _resolver = new SwaggerToCSharpTypeResolver(_service.Definitions);
        }

        /// <summary>Gets or sets the generator settings.</summary>
        public SwaggerToCSharpGeneratorSettings Settings { get; set; }

        /// <summary>Gets the language.</summary>
        protected override string Language
        {
            get { return "CSharp"; }
        }

        /// <summary>Generates the file.</summary>
        /// <returns>The file contents.</returns>
        public override string GenerateFile()
        {
            return GenerateFile(_service, _resolver);
        }

        internal override ClientGeneratorBaseSettings BaseSettings
        {
            get { return Settings; }
        }

        internal override string RenderFile(string clientCode)
        {
            var template = LoadTemplate("File");
            template.Add("namespace", Settings.Namespace);
            template.Add("toolchain", SwaggerService.ToolchainVersion);
            template.Add("clients", Settings.GenerateClientTypes ? clientCode : string.Empty);
            template.Add("namespaceUsages", Settings.AdditionalNamespaceUsages);
            template.Add("classes", Settings.GenerateDtoTypes ? _resolver.GenerateTypes() : string.Empty);
            return template.Render();
        }

        internal override string RenderClientCode(string controllerName, IEnumerable<OperationModel> operations)
        {
            var template = LoadTemplate("Client");
            template.Add("class", Settings.ClassName.Replace("{controller}", ConvertToUpperStartIdentifier(controllerName)));

            var hasClientBaseClass = !string.IsNullOrEmpty(Settings.ClientBaseClass); 
            template.Add("clientBaseClass", Settings.ClientBaseClass);
            template.Add("hasClientBaseClass", hasClientBaseClass);

            template.Add("useHttpClientCreationMethod", Settings.UseHttpClientCreationMethod);
            template.Add("generateClientInterfaces", Settings.GenerateClientInterfaces);
            template.Add("hasBaseType", Settings.GenerateClientInterfaces || hasClientBaseClass);

            template.Add("baseUrl", _service.BaseUrl);
            template.Add("operations", operations);
            template.Add("hasOperations", operations.Any());

            return template.Render();
        }

        internal override string GetExceptionType(SwaggerOperation operation)
        {
            if (operation.Responses.Count(r => r.Key != "200") != 1)
                return "Exception";

            return GetType(operation.Responses.Single(r => r.Key != "200").Value.Schema, "Exception");
        }

        internal override string GetResultType(SwaggerOperation operation)
        {
            if (operation.Responses.Count(r => r.Key == "200") == 0)
                return "Task";

            var response = GetOkResponse(operation);
            return "Task<" + GetType(response.Schema, "Response") + ">";
        }

        internal override string GetType(JsonSchema4 schema, string typeNameHint)
        {
            if (schema == null)
                return "string";

            if (schema.ActualSchema.IsAnyType)
                return "object";

            return _resolver.Resolve(schema.ActualSchema, true, typeNameHint);
        }
    }
}
