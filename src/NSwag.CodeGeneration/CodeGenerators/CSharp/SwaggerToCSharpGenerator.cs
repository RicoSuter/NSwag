//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

namespace NSwag.CodeGeneration.CodeGenerators.CSharp
{
    /// <summary>The CSharp generator base class.</summary>
    public abstract class SwaggerToCSharpGenerator : ClientGeneratorBase
    {
        internal SwaggerToCSharpTypeResolver Resolver { get; private set; }

        internal SwaggerToCSharpGenerator(SwaggerService service, CSharpGeneratorSettings settings)
        {
            Resolver = new SwaggerToCSharpTypeResolver(settings, service.Definitions);
        }

        internal override string GetExceptionType(SwaggerOperation operation)
        {
            if (operation.Responses.Count(r => !HttpUtilities.IsSuccessStatusCode(r.Key)) != 1)
                return "Exception";

            return GetType(operation.Responses.Single(r => !HttpUtilities.IsSuccessStatusCode(r.Key)).Value.Schema, "Exception");
        }

        internal override string GetResultType(SwaggerOperation operation)
        {
            var response = GetSuccessResponse(operation);
            if (response == null)
                return "Task";

            return "Task<" + GetType(response.Schema, "Response") + ">";
        }

        internal override string GetType(JsonSchema4 schema, string typeNameHint)
        {
            if (schema == null)
                return "void";

            if (schema.ActualSchema.IsAnyType)
                return "object";

            return Resolver.Resolve(schema.ActualSchema, true, typeNameHint);
        }
    }
}