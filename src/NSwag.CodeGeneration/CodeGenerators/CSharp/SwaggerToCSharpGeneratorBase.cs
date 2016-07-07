//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpGeneratorBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NSwag.CodeGeneration.CodeGenerators.CSharp.Templates;

namespace NSwag.CodeGeneration.CodeGenerators.CSharp
{
    /// <summary>The CSharp generator base class.</summary>
    public abstract class SwaggerToCSharpGeneratorBase : ClientGeneratorBase
    {
        private readonly SwaggerToCSharpGeneratorSettings _settings;
        private readonly SwaggerService _service;

        internal SwaggerToCSharpTypeResolver Resolver { get; private set; }

        internal SwaggerToCSharpGeneratorBase(SwaggerService service, SwaggerToCSharpGeneratorSettings settings)
        {
            _service = service;
            _settings = settings;

            Resolver = new SwaggerToCSharpTypeResolver(settings.CSharpGeneratorSettings, service.Definitions);
        }

        internal override string GenerateFile(string clientCode, IEnumerable<string> clientClasses, ClientGeneratorOutputType outputType)
        {
            var generateOnlyContracts = outputType == ClientGeneratorOutputType.Contracts;
            var generateImplementation = outputType == ClientGeneratorOutputType.Full || outputType == ClientGeneratorOutputType.Implementation;

            var template = new FileTemplate();
            template.Initialize(new // TODO: Add typed class
            {
                Namespace = _settings.CSharpGeneratorSettings.Namespace ?? string.Empty,
                NamespaceUsages = generateOnlyContracts || _settings.AdditionalNamespaceUsages == null ? new string[] { } : _settings.AdditionalNamespaceUsages,

                GenerateContracts = outputType == ClientGeneratorOutputType.Full || outputType == ClientGeneratorOutputType.Contracts,
                GenerateImplementation = generateImplementation,

                Clients = _settings.GenerateClientClasses ? clientCode : string.Empty,
                Classes = _settings.GenerateDtoTypes ? Resolver.GenerateClasses() : string.Empty,

                HasMissingHttpMethods = _service.Operations.Any(o =>
                    o.Method == SwaggerOperationMethod.Options ||
                    o.Method == SwaggerOperationMethod.Head ||
                    o.Method == SwaggerOperationMethod.Patch)
            });

            return template.Render();
        }
        
        internal override string GetExceptionType(SwaggerOperation operation)
        {
            if (operation.Responses.Count(r => !HttpUtilities.IsSuccessStatusCode(r.Key)) != 1)
                return "Exception";

            var response = operation.Responses.Single(r => !HttpUtilities.IsSuccessStatusCode(r.Key)).Value;
            return GetType(response.ActualResponseSchema, response.IsNullable(BaseSettings.CodeGeneratorSettings.NullHandling), "Exception");
        }

        internal override string GetResultType(SwaggerOperation operation)
        {
            var response = GetSuccessResponse(operation);
            if (response?.Schema == null)
                return "Task";

            return "Task<" + GetType(response.ActualResponseSchema, response.IsNullable(BaseSettings.CodeGeneratorSettings.NullHandling), "Response") + ">";
        }

        internal override string GetType(JsonSchema4 schema, bool isNullable, string typeNameHint)
        {
            if (schema == null)
                return "void";

            if (schema.ActualSchema.Type == JsonObjectType.File)
                return "byte[]";

            if (schema.ActualSchema.IsAnyType)
                return "object";

            return Resolver.Resolve(schema.ActualSchema, isNullable, typeNameHint);
        }
    }
}