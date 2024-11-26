//-----------------------------------------------------------------------
// <copyright file="IOperationModel.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.Models
{
    /// <summary>The operation model interface.</summary>
    public interface IOperationModel
    {
        /// <summary>Gets the responses.</summary>
        IEnumerable<ResponseModelBase> Responses { get; }

        /// <summary>Gets Swagger operation's mime type.</summary>
        string Produces { get; }
    }
}