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
        private static readonly string[] ReservedKeywords = new[]
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue",
            "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float",
            "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object",
            "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof",
            "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
            "ushort", "using", "virtual", "void", "volatile", "while"
        };

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
                return "System.Exception";

            var response = operation.Responses.Single(r => !HttpUtilities.IsSuccessStatusCode(r.Key)).Value;
            return GetType(response.ActualResponseSchema, response.IsNullable(BaseSettings.CodeGeneratorSettings.NullHandling), "Exception");
        }

        internal override string GetResultType(SwaggerOperation operation)
        {
            var response = GetSuccessResponse(operation);
            if (response?.ActualResponseSchema == null)
                return "void";

            return GetType(response.ActualResponseSchema, response.IsNullable(BaseSettings.CodeGeneratorSettings.NullHandling), "Response");
        }

        internal override string GetParameterVariableName(SwaggerParameter parameter, IEnumerable<SwaggerParameter> allParameters)
        {
            var name = base.GetParameterVariableName(parameter, allParameters);
            return ReservedKeywords.Contains(name) ? "@" + name : name;
        }

        internal override string GetType(JsonSchema4 schema, bool isNullable, string typeNameHint)
        {
            if (schema == null)
                return "void";

            if (schema.ActualSchema.Type == JsonObjectType.File)
                return "FileResponse";

            if (schema.ActualSchema.IsAnyType)
                return "object";

            return Resolver.Resolve(schema.ActualSchema, isNullable, typeNameHint);
        }
    }
}