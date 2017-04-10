//-----------------------------------------------------------------------
// <copyright file="IOperationModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace NSwag.CodeGeneration.Models
{
    /// <summary>The operation model interface.</summary>
    public interface IOperationModel
    {
        /// <summary>Gets the responses.</summary>
        IEnumerable<ResponseModelBase> Responses { get; }
    }
}