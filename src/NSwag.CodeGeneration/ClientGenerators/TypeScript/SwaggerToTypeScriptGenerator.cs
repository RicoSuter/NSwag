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

        /// <summary>Initializes a new instance of the <see cref="SwaggerToTypeScriptGenerator" /> class.</summary>
        /// <param name="service">The service.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.ArgumentNullException">service</exception>
        /// <exception cref="ArgumentNullException"><paramref name="service" /> is <see langword="null" />.</exception>
        public SwaggerToTypeScriptGenerator(SwaggerService service, SwaggerToTypeScriptGeneratorSettings settings)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            Settings = settings; 

            _service = service;
            foreach (var definition in _service.Definitions)
                definition.Value.TypeName = definition.Key;

            _resolver = new TypeScriptTypeResolver(_service.Definitions.Select(p => p.Value).ToArray());
        }

        /// <summary>Gets or sets the generator settings.</summary>
        public SwaggerToTypeScriptGeneratorSettings Settings { get; set; }

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

        internal override ClientGeneratorBaseSettings BaseSettings
        {
            get { return Settings; }
        }

        internal override string RenderFile(string clientCode)
        {
            var template = LoadTemplate("File");
            template.Add("toolchain", SwaggerService.ToolchainVersion);
            template.Add("clients", Settings.GenerateClientClasses ? clientCode : string.Empty);
            template.Add("interfaces", Settings.GenerateDtoTypes ? _resolver.GenerateTypes() : string.Empty);
            return template.Render();
        }

        internal override string RenderClientCode(string controllerName, IEnumerable<OperationModel> operations)
        {
            var template = LoadTemplate(Settings.AsyncType == TypeScriptAsyncType.Callbacks ? "Callbacks" : "Q");
            template.Add("class", Settings.Class.Replace("{controller}", ConvertToUpperStartIdentifier(controllerName)));
            template.Add("operations", operations);
            template.Add("generateClientInterfaces", Settings.GenerateClientInterfaces);
            template.Add("hasOperations", operations.Any());
            template.Add("baseUrl", _service.BaseUrl);
            return template.Render();
        }

        internal override string GetExceptionType(SwaggerOperation operation)
        {
            if (operation.Responses.Count(r => r.Key != "200") == 0)
                return "string";

            return string.Join(" | ", operation.Responses
                .Where(r => r.Key != "200")
                .Select(r => GetType(r.Value.Schema.ActualSchema, "Exception"))) + " | string";
        }

        internal override string GetResultType(SwaggerOperation operation)
        {
            if (operation.Responses.Count(r => r.Key == "200") == 0)
                return "void";

            var response = GetOkResponse(operation);
            return GetType(response.Schema, "Response");
        }

        internal override string GetType(JsonSchema4 schema, string typeNameHint)
        {
            if (schema == null)
                return "any";

            if (schema.ActualSchema.IsAnyType)
                return "any";

            return _resolver.Resolve(schema.ActualSchema, true, typeNameHint);
        }
    }
}
