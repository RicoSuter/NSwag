//-----------------------------------------------------------------------
// <copyright file="ICodeGeneratorView.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;

namespace NSwagStudio
{
    /// <summary>The interface for a client generator.</summary>
    public interface ICodeGeneratorView
    {
        /// <summary>Gets the title.</summary>
        string Title { get; }

        /// <summary>Generates the client code.</summary>
        Task GenerateClientAsync(string swaggerData, string documentPath);
    }
}