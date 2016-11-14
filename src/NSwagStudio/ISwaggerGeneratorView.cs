//-----------------------------------------------------------------------
// <copyright file="ISwaggerGeneratorView.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using NSwag.Commands.Base;

namespace NSwagStudio
{
    /// <summary>The interface for a Swagger generator.</summary>
    public interface ISwaggerGeneratorView
    {
        /// <summary>Gets the title.</summary>
        string Title { get; }

        /// <summary>Gets the command.</summary>
        OutputCommandBase Command { get; }

        /// <summary>Generates the Swagger specification.</summary>
        Task<string> GenerateSwaggerAsync();
    }
}