//-----------------------------------------------------------------------
// <copyright file="OperationGenerationMode.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.ClientGenerators
{
    /// <summary>Specifies how the operation name and client classes/interfaces are generated.</summary>
    public enum OperationGenerationMode
    {
        /// <summary>From the Swagger operation ID.</summary>
        SingleClientFromOperationId,

        /// <summary>From path segments (operation name = nth segment, client name = nth - 1 segment).</summary>
        MultipleClientsFromPathSegments
    }
}