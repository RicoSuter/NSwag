//-----------------------------------------------------------------------
// <copyright file="OperationGenerationMode.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.CodeGenerators
{
    /// <summary>Specifies how the operation name and client classes/interfaces are generated.</summary>
    public enum OperationGenerationMode
    {
        /// <summary>From the Swagger operation ID.</summary>
        SingleClientFromOperationId,

        /// <summary>From path segments (operation name = last segment, client name = second to last segment).</summary>
        MultipleClientsFromPathSegments
    }
}