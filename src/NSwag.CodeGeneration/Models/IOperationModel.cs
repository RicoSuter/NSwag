//-----------------------------------------------------------------------
// <copyright file="IOperationModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.Models
{
    /// <summary>The operation model interface.</summary>
    public interface IOperationModel
    {
        /// <summary>Gets a value indicating whether the operation has an explicit success response defined.</summary>
        bool HasSuccessResponse { get; }

        /// <summary>Gets the success response.</summary>
        ResponseModelBase SuccessResponse { get; }
    }
}