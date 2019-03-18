//-----------------------------------------------------------------------
// <copyright file="IClientGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;

namespace NSwag.CodeGeneration
{
    /// <summary>The client generator interface.</summary>
    public interface IClientGenerator
    {
        /// <summary>Gets the type.</summary>
        /// <param name="schema">The schema.</param>
        /// <param name="isNullable">if set to <c>true</c> [is nullable].</param>
        /// <param name="typeNameHint">The type name hint.</param>
        /// <returns>The type name.</returns>
        string GetTypeName(JsonSchema4 schema, bool isNullable, string typeNameHint);

        /// <summary>Gets the file/binary response type name.</summary>
        /// <returns>The type name.</returns>
        string GetBinaryResponseTypeName();
    }
}