//-----------------------------------------------------------------------
// <copyright file="SwaggerToTypeScriptClientGenerator.cs" company="NSwag">
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
using NSwag.CodeGeneration.CodeGenerators.Models;

namespace NSwag.CodeGeneration.CodeGenerators.TypeScript
{
    /// <summary>Generates the CSharp service client code. </summary>
    public class SwaggerToTypeScriptClientGenerator : ClientGeneratorBase
    {
        private readonly SwaggerService _service;
        private readonly TypeScriptTypeResolver _resolver;

        /// <summary>Initializes a new instance of the <see cref="SwaggerToTypeScriptClientGenerator" /> class.</summary>
        /// <param name="service">The service.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.ArgumentNullException">service</exception>
        /// <exception cref="ArgumentNullException"><paramref name="service" /> is <see langword="null" />.</exception>
        public SwaggerToTypeScriptClientGenerator(SwaggerService service, SwaggerToTypeScriptClientGeneratorSettings settings)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            Settings = settings;

            _service = service;
            foreach (var definition in _service.Definitions)
                definition.Value.TypeName = definition.Key;

            _resolver = new TypeScriptTypeResolver(_service.Definitions.Select(p => p.Value).ToArray());
        }

        /// <summary>Gets or sets the generator settings.</summary>
        public SwaggerToTypeScriptClientGeneratorSettings Settings { get; set; }

        /// <summary>Gets the language.</summary>
        protected override string Language => "TypeScript";

        /// <summary>Generates the file.</summary>
        /// <returns>The file contents.</returns>
        public override string GenerateFile()
        {
            return GenerateFile(_service, _resolver);
        }

        internal override CodeGeneratorBaseSettings BaseSettings => Settings;

        internal override string RenderFile(string clientCode)
        {
            var template = LoadTemplate("File");
            template.Add("toolchain", SwaggerService.ToolchainVersion);
            template.Add("isAngular2", Settings.Template == TypeScriptTemplate.Angular2);
            template.Add("clients", Settings.GenerateClientClasses ? clientCode : string.Empty);
            template.Add("interfaces", Settings.GenerateDtoTypes ? _resolver.GenerateTypes() : string.Empty);
            template.Add("hasModuleName", !string.IsNullOrEmpty(Settings.ModuleName));
            template.Add("moduleName", Settings.ModuleName);
            return template.Render();
        }

        internal override string RenderClientCode(string controllerName, IEnumerable<OperationModel> operations)
        {
            var template = LoadTemplate(Settings.Template.ToString());

            template.Add("class", Settings.ClassName.Replace("{controller}", ConvertToUpperCamelCase(controllerName)));
            template.Add("operations", operations);
            template.Add("generateClientInterfaces", Settings.GenerateClientInterfaces);
            template.Add("hasOperations", operations.Any());
            template.Add("baseUrl", _service.BaseUrl);

            template.Add("promiseType", Settings.PromiseType == PromiseType.Promise ? "Promise" : "Q.Promise");
            template.Add("promiseConstructor", Settings.PromiseType == PromiseType.Promise ? "new Promise" : "Q.Promise");

            return template.Render();
        }

        internal override string GetExceptionType(SwaggerOperation operation)
        {
            if (operation.Responses.Count(r => !HttpUtilities.IsSuccessStatusCode(r.Key)) == 0)
                return "string";

            return string.Join(" | ", operation.Responses
                .Where(r => !HttpUtilities.IsSuccessStatusCode(r.Key) && r.Value.Schema != null)
                .Select(r => GetType(r.Value.Schema.ActualSchema, "Exception"))
                .Concat(new[] { "string" }));
        }

        internal override string GetResultType(SwaggerOperation operation)
        {
            var response = GetSuccessResponse(operation);
            if (response?.Schema == null)
                return "void";

            return GetType(response.Schema, "Response");
        }

        internal override string GetType(JsonSchema4 schema, string typeNameHint)
        {
            if (schema == null)
                return "void";

            if (schema.ActualSchema.IsAnyType || schema.ActualSchema.Type == JsonObjectType.File)
                return "any";

            return _resolver.Resolve(schema.ActualSchema, schema.ActualSchema.Type.HasFlag(JsonObjectType.Null), typeNameHint);
        }
    }
}
