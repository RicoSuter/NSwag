//-----------------------------------------------------------------------
// <copyright file="AssemblyTypeToSwaggerCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using NSwag.SwaggerGeneration;

namespace NSwag.Commands
{
    /// <summary></summary>
    /// <seealso cref="NSwag.Commands.AssemblyTypeToSwaggerCommandBase" />
    public class AssemblyTypeToSwaggerCommand : AssemblyTypeToSwaggerCommandBase
    {
        /// <summary>Creates a new generator instance.</summary>
        /// <returns>The generator.</returns>
        protected override Task<AssemblyTypeToSwaggerGeneratorBase> CreateGeneratorAsync()
        {
            return Task.FromResult<AssemblyTypeToSwaggerGeneratorBase>(new AssemblyTypeToSwaggerGenerator(Settings));
        }
    }
}