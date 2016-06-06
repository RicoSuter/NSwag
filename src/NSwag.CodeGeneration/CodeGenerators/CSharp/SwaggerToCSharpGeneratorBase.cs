//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpGeneratorBase.cs" company="NSwag">
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
    public abstract class SwaggerToCSharpGeneratorBase : ClientGeneratorBase
    {
        internal SwaggerToCSharpTypeResolver Resolver { get; private set; }

        internal SwaggerToCSharpGeneratorBase(SwaggerService service, CSharpGeneratorSettings settings)
        {
            Resolver = new SwaggerToCSharpTypeResolver(settings, service.Definitions);
        }

        internal override string GetExceptionType(SwaggerOperation operation)
        {
            if (operation.Responses.Count(r => !HttpUtilities.IsSuccessStatusCode(r.Key)) != 1)
                return "Exception";

            var response = operation.Responses.Single(r => !HttpUtilities.IsSuccessStatusCode(r.Key)).Value; 
            return GetType(response.ActualResponseSchema, response.IsNullable(PropertyNullHandling.Required), "Exception");
        }

        internal override string GetResultType(SwaggerOperation operation)
        {
            var response = GetSuccessResponse(operation);
            if (response?.Schema == null)
                return "Task";

            return "Task<" + GetType(response.ActualResponseSchema, response.IsNullable(PropertyNullHandling.Required), "Response") + ">";
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