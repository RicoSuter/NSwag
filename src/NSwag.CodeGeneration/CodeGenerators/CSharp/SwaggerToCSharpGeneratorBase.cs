//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpGeneratorBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NSwag.CodeGeneration.CodeGenerators.CSharp.Models;

namespace NSwag.CodeGeneration.CodeGenerators.CSharp
{
    /// <summary>The CSharp generator base class.</summary>
    public abstract class SwaggerToCSharpGeneratorBase : ClientGeneratorBase
    {
        private readonly SwaggerToCSharpGeneratorSettings _settings;
        private readonly SwaggerDocument _document;

        internal SwaggerToCSharpGeneratorBase(SwaggerDocument document, SwaggerToCSharpGeneratorSettings settings, SwaggerToCSharpTypeResolver resolver)
            : base(resolver, settings.CodeGeneratorSettings)
        {
            _document = document;
            _settings = settings;
        }

        internal override string GenerateFile(string clientCode, IEnumerable<string> clientClasses, ClientGeneratorOutputType outputType)
        {
            var model = new FileTemplateModel(clientCode, outputType, _document, this, _settings, (SwaggerToCSharpTypeResolver)Resolver);
            var template = _settings.CodeGeneratorSettings.TemplateFactory.CreateTemplate("CSharp", "File", model);
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