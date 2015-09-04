//-----------------------------------------------------------------------
// <copyright file="ISwaggerGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;

namespace NSwagStudio
{
    public interface ISwaggerGenerator
    {
        string Title { get; }

        Task<string> GenerateSwaggerAsync();
    }
}