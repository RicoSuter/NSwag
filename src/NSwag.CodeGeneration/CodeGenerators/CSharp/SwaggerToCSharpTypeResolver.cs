//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpTypeResolver.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

namespace NSwag.CodeGeneration.CodeGenerators.CSharp
{
    /// <summary>A resolver which returns Exception without generating the class (uses System.Exception instead of own class).</summary>
    internal class SwaggerToCSharpTypeResolver : CSharpTypeResolver
    {
        private readonly JsonSchema4 _exceptionSchema;

        /// <summary>Initializes a new instance of the <see cref="CSharpTypeResolver" /> class.</summary>
        /// <param name="settings">The generator settings.</param>
        /// <param name="exceptionSchema">The exception type schema.</param>
        public SwaggerToCSharpTypeResolver(CSharpGeneratorSettings settings, JsonSchema4 exceptionSchema)
            : base(settings)
        {
            _exceptionSchema = exceptionSchema;
        }

        /// <summary>Resolves and possibly generates the specified schema.</summary>
        /// <param name="schema">The schema.</param>
        /// <param name="isNullable">Specifies whether the given type usage is nullable.</param>
        /// <param name="typeNameHint">The type name hint to use when generating the type and the type name is missing.</param>
        /// <returns>The type name.</returns>
        public override string Resolve(JsonSchema4 schema, bool isNullable, string typeNameHint)
        {
            if (schema.ActualSchema == _exceptionSchema)
                return "Exception";

            return base.Resolve(schema, isNullable, typeNameHint);
        }
    }
}