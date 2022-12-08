//-----------------------------------------------------------------------
// <copyright file="OperationGenerationMode.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.Commands.CodeGeneration
{
    /// <summary>Specifies how the operation name and client classes/interfaces are generated.</summary>
    public enum OperationGenerationMode
    {
        /// <summary>Multiple clients from the Swagger operation ID in the form '{controller}_{action}'.</summary>
        MultipleClientsFromOperationId,

        /// <summary>From path segments (operation name = last segment, client name = second to last segment).</summary>
        MultipleClientsFromPathSegments,

        /// <summary>From the first operation tag and path segments (operation name = last segment, client name = first operation tag).</summary>
        MultipleClientsFromFirstTagAndPathSegments,

        /// <summary>From the first operation tag and operation ID (operation name = operation ID, client name = first operation tag).</summary>
        MultipleClientsFromFirstTagAndOperationId,

        /// <summary>From the Swagger operation ID.</summary>
        SingleClientFromOperationId,

        /// <summary>From path segments suffixed by HTTP operation name</summary>
        SingleClientFromPathSegments,

        /// <summary>From the first operation tag and operation name (underscore separated from operation id)</summary>
        MultipleClientsFromFirstTagAndOperationName,
    }
}
