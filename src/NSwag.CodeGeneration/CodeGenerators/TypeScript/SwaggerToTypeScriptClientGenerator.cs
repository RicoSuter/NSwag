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
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.CodeGenerators.Models;
using NSwag.CodeGeneration.CodeGenerators.TypeScript.Templates;

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

            _resolver = new TypeScriptTypeResolver(_service.Definitions.Select(p => p.Value).ToArray(), Settings.TypeScriptGeneratorSettings);
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
            var template = new FileTemplate();
            template.Initialize(new
            {
                Toolchain = SwaggerService.ToolchainVersion, 
                IsAngular2 = Settings.GenerateClientClasses && Settings.Template == TypeScriptTemplate.Angular2, 

                Clients = Settings.GenerateClientClasses ? clientCode : string.Empty, 
                Interfaces = Settings.GenerateDtoTypes ? _resolver.GenerateTypes() : string.Empty,

                HasModuleName = !string.IsNullOrEmpty(Settings.ModuleName), 
                ModuleName = Settings.ModuleName
            });
            return template.Render();
        }

        internal override string RenderClientCode(string controllerName, IEnumerable<OperationModel> operations)
        {
            GenerateDataConversionCodes(operations);

            var template = Settings.CreateTemplate();
            template.Initialize(new
            {
                Class = Settings.ClassName.Replace("{controller}", ConversionUtilities.ConvertToUpperCamelCase(controllerName)),

                HasOperations = operations.Any(),
                Operations = operations,

                GenerateClientInterfaces = Settings.GenerateClientInterfaces,
                BaseUrl = _service.BaseUrl, 
                UseDtoClasses = Settings.TypeScriptGeneratorSettings.TypeStyle != TypeScriptTypeStyle.Interface,

                PromiseType = Settings.PromiseType == PromiseType.Promise ? "Promise" : "Q.Promise",
                PromiseConstructor = Settings.PromiseType == PromiseType.Promise ? "new Promise" : "Q.Promise"
            });
            
            return template.Render();
        }

        private void GenerateDataConversionCodes(IEnumerable<OperationModel> operations)
        {
            foreach (var operation in operations)
            {
                foreach (var response in operation.Responses.Where(r => r.HasType))
                {
                    var generator = new TypeScriptGenerator(response.Schema, Settings.TypeScriptGeneratorSettings, _resolver);
                    response.DataConversionCode = generator.GenerateDataConversion("result" + response.StatusCode,
                        "resultData" + response.StatusCode, response.Schema, false, string.Empty);
                }

                if (operation.HasDefaultResponse && operation.DefaultResponse.HasType)
                {
                    var generator = new TypeScriptGenerator(operation.DefaultResponse.Schema,
                        Settings.TypeScriptGeneratorSettings, _resolver);
                    operation.DefaultResponse.DataConversionCode = generator.GenerateDataConversion("result", "resultData",
                        operation.DefaultResponse.Schema, false, string.Empty);
                }
            }
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
